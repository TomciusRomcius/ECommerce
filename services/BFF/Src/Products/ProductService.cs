using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Amazon.S3;
using Amazon.S3.Transfer;
using BFF.Configuration;
using BFF.Utils;
using ECommerceBackend.Utils.Microservices;
using ECommerceBackend.Utils.Pagination;
using Microsoft.Extensions.Options;

namespace BFF.Products;

internal sealed class CreateProductResponseDto
{
    public int ProductId { get; set; }
}

public class ProductService(
    HttpClient httpClient,
    IOptions<MicroserviceHosts> hosts,
    IOptions<S3Configuration> s3Configuration,
    IAmazonS3 s3Client,
    IS3ImageUrlBuilder s3ImageUrlBuilder,
    ILogger<ProductService> logger) : IProductService
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public async Task<Result<Page<ProductWithImageUrlsDto>>> GetProductsAsync(
        string searchText,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = new QueryString()
            .Add("searchText", searchText.ToString())
            .Add("pageNumber", pageNumber.ToString())
            .Add("pageSize", pageSize.ToString());
        string upstreamUrl = $"{hosts.Value.ProductServiceUrl}/Product{query}";

        using HttpResponseMessage response = await httpClient.GetAsync(upstreamUrl, cancellationToken);
        string body = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return new Result<Page<ProductWithImageUrlsDto>>([
                new ResultError(ResultErrorType.UPSTREAM_ERROR, body, (int)response.StatusCode),
            ]);
        }

        Page<ProductWithImageKeysDto>? page =
            JsonSerializer.Deserialize<Page<ProductWithImageKeysDto>>(body, JsonOptions);

        if (page is null)
        {
            return new Result<Page<ProductWithImageUrlsDto>>([
                new ResultError(ResultErrorType.GATEWAY_ERROR, "Failed to deserialize product page."),
            ]);
        }

        Page<ProductWithImageUrlsDto> mapped = new()
        {
            Data = page.Data.Select(ToProductWithImageUrls).ToList(),
            TotalCount = page.TotalCount,
            PageSize = page.PageSize,
            HasNextPage = page.HasNextPage,
            HasPrevPage = page.HasPrevPage,
        };

        return new Result<Page<ProductWithImageUrlsDto>>(mapped);
    }

    public async Task<Result<ProductWithImageUrlsDto>> GetProductAsync(
        int productId,
        CancellationToken cancellationToken = default)
    {
        var query = new QueryString().Add("ids", productId.ToString());
        string upstreamUrl = $"{hosts.Value.ProductServiceUrl}/Product/by-ids{query}";

        using HttpResponseMessage response = await httpClient.GetAsync(upstreamUrl, cancellationToken);
        string body = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return new Result<ProductWithImageUrlsDto>([
                new ResultError(ResultErrorType.UPSTREAM_ERROR, body, (int)response.StatusCode),
            ]);
        }

        List<ProductWithImageKeysDto>? products =
            JsonSerializer.Deserialize<List<ProductWithImageKeysDto>>(body, JsonOptions);

        ProductWithImageKeysDto? product = products?.FirstOrDefault();
        if (product is null)
        {
            return new Result<ProductWithImageUrlsDto>([
                new ResultError(ResultErrorType.NOT_FOUND, $"Product {productId} was not found."),
            ]);
        }

        return new Result<ProductWithImageUrlsDto>(ToProductWithImageUrls(product));
    }

    public async Task<HttpResponseMessage> CreateProductAsync(
        CreateProductRequest request,
        string? authorizationHeader,
        CancellationToken cancellationToken = default)
    {
        string upstreamUrl = $"{hosts.Value.ProductServiceUrl}/Product";
        logger.LogDebug("Creating product at {Url}", upstreamUrl);

        using var upstreamRequest = new HttpRequestMessage(HttpMethod.Post, upstreamUrl);
        if (!string.IsNullOrWhiteSpace(authorizationHeader))
        {
            upstreamRequest.Headers.Authorization = AuthenticationHeaderValue.Parse(authorizationHeader);
        }

        upstreamRequest.Content = JsonContent.Create(new
        {
            name = request.Name,
            description = request.Description,
            price = request.Price,
            manufacturerId = request.ManufacturerId,
            categoryId = request.CategoryId,
            imageKeys = Array.Empty<string>(),
        });

        using HttpResponseMessage createResponse =
            await httpClient.SendAsync(upstreamRequest, cancellationToken);

        string createBody = await createResponse.Content.ReadAsStringAsync(cancellationToken);

        if (!createResponse.IsSuccessStatusCode)
        {
            logger.LogWarning(
                "Create product failed with status {StatusCode}: {Body}",
                createResponse.StatusCode,
                createBody);
            return createResponse;
        }

        CreateProductResponseDto? created =
            JsonSerializer.Deserialize<CreateProductResponseDto>(createBody, JsonOptions);

        if (created is null)
        {
            logger.LogWarning("Create product succeeded but response could not be deserialized: {Body}", createBody);
            return createResponse;
        }

        if (request.Files.Count == 0)
        {
            return createResponse;
        }

        int productId = created.ProductId;
        string bucketName = s3Configuration.Value.BucketName;
        var transferUtility = new TransferUtility(s3Client);

        string[] keys = await Task.WhenAll(request.Files.Select((file, index) => UploadImageAsync(
            file,
            bucketName,
            transferUtility,
            $"{productId}_{index}",
            cancellationToken)));

        using var addImagesRequest = new HttpRequestMessage(
            HttpMethod.Post,
            $"{upstreamUrl}/{productId}/images");
        if (!string.IsNullOrWhiteSpace(authorizationHeader))
        {
            addImagesRequest.Headers.Authorization = AuthenticationHeaderValue.Parse(authorizationHeader);
        }

        addImagesRequest.Content = JsonContent.Create(new { imageKeys = keys });

        HttpResponseMessage addImagesResponse = await httpClient.SendAsync(addImagesRequest, cancellationToken);

        if (!addImagesResponse.IsSuccessStatusCode)
        {
            string addImagesBody = await addImagesResponse.Content.ReadAsStringAsync(cancellationToken);
            logger.LogWarning(
                "Add product images failed for product {ProductId} with status {StatusCode}: {Body}",
                productId,
                addImagesResponse.StatusCode,
                addImagesBody);
            return addImagesResponse;
        }

        return createResponse;
    }

    private ProductWithImageUrlsDto ToProductWithImageUrls(ProductWithImageKeysDto product)
    {
        IReadOnlyList<string> sortedKeys = SortImageKeysByFileOrder(product.ImageKeys);
        return new ProductWithImageUrlsDto
        {
            ProductId = product.ProductId,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            ManufacturerId = product.ManufacturerId,
            CategoryId = product.CategoryId,
            Manufacturer = product.Manufacturer,
            Category = product.Category,
            ImageUrls = s3ImageUrlBuilder.BuildUrls(sortedKeys),
        };
    }

    private static IReadOnlyList<string> SortImageKeysByFileOrder(IEnumerable<string> imageKeys) =>
        imageKeys
            .Where(key => !string.IsNullOrWhiteSpace(key))
            .OrderBy(GetFileOrderFromKey)
            .ToList();

    private static int GetFileOrderFromKey(string key)
    {
        int underscoreIndex = key.LastIndexOf('_');
        if (underscoreIndex < 0 || underscoreIndex >= key.Length - 1)
        {
            return int.MaxValue;
        }

        return int.TryParse(key.AsSpan(underscoreIndex + 1), out int order) ? order : int.MaxValue;
    }

    private static async Task<string> UploadImageAsync(
        IFormFile file,
        string bucketName,
        TransferUtility transferUtility,
        string key,
        CancellationToken cancellationToken)
    {
        using var fileStream = new MemoryStream();
        await file.CopyToAsync(fileStream, cancellationToken);
        fileStream.Position = 0;

        var uploadRequest = new TransferUtilityUploadRequest
        {
            BucketName = bucketName,
            Key = key,
            InputStream = fileStream,
        };
        await transferUtility.UploadAsync(uploadRequest, cancellationToken);
        return key;
    }
}

