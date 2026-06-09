using System.Text.Json;
using BFF.Configuration;
using BFF.ReadDb;
using BFF.ReadDb.Entities;
using BFF.Utils;
using ECommerceBackend.Utils.Microservices;
using ECommerceBackend.Utils.Pagination;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BFF.StoreProducts;

public class StoreProductsService(
    IHttpClientFactory httpClientFactory,
    ReadDbContext readDbContext,
    IOptions<MicroserviceHosts> hosts,
    IOptions<S3Configuration> s3Configuration,
    ILogger<StoreProductsService> logger) : IStoreProductsService
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient();
    private readonly S3Configuration _s3Configuration = s3Configuration.Value;

    public async Task<Page<StoreProductDto>> GetProductsAsync(
        int? storeLocationId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        IQueryable<StoreProductReadEntity> storeProductsQuery = readDbContext.StoreProducts.AsNoTracking();

        if (storeLocationId is not null)
        {
            storeProductsQuery = storeProductsQuery.Where(product => product.StoreLocationId == storeLocationId.Value);
        }

        int totalCount = await storeProductsQuery.CountAsync(cancellationToken);

        List<StoreProductReadEntity> rows = await storeProductsQuery
            .Include(p => p.ProductImages)
            .OrderBy(p => p.ProductId)
            .ThenBy(p => p.StoreLocationId)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .GroupBy(p => p.ProductId)
            .Select(g => g.First())
            .ToListAsync(cancellationToken);

        List<StoreProductDto> data = rows.Select(MapToDto).ToList();

        int fetchedCount = (pageNumber * pageSize) + data.Count;
        return new Page<StoreProductDto>
        {
            Data = data,
            TotalCount = totalCount,
            HasNextPage = fetchedCount < totalCount,
            HasPrevPage = pageNumber > 0,
            PageSize = pageSize,
        };
    }

    public async Task<StoreProductDto?> GetProductByIdAsync(
        int productId,
        CancellationToken cancellationToken = default)
    {
        StoreProductReadEntity? product = await readDbContext.StoreProducts
            .AsNoTracking()
            .Include(storeProduct => storeProduct.ProductImages)
            .Where(storeProduct => storeProduct.ProductId == productId)
            .OrderBy(storeProduct => storeProduct.StoreLocationId)
            .FirstOrDefaultAsync(cancellationToken);

        return product is null ? null : MapToDto(product);
    }

    public async Task<HttpResponseMessage> UpdateProductStockAsync(
        int storeLocationId,
        int productId,
        int stock,
        string? authorizationHeader,
        CancellationToken cancellationToken = default)
    {
        string url = $"{hosts.Value.StoreServiceUrl}/availableproducts";
        logger.LogDebug("Updating product stock at {Url}", url);

        using var request = new HttpRequestMessage(HttpMethod.Put, url);
        HttpRequestUtils.ApplyAuthorizationHeader(request, authorizationHeader);
        request.Content = JsonContent.Create(new
        {
            storeLocationId,
            productId,
            stock,
        });

        return await _httpClient.SendAsync(request, cancellationToken);
    }

    public async Task<HttpResponseMessage> AddProductToStoreAsync(
        int storeLocationId,
        int productId,
        int stock,
        string? authorizationHeader,
        CancellationToken cancellationToken = default)
    {
        string url = $"{hosts.Value.StoreServiceUrl}/availableproducts";
        logger.LogDebug("Adding product to store at {Url}", url);

        using var request = new HttpRequestMessage(HttpMethod.Post, url);
        HttpRequestUtils.ApplyAuthorizationHeader(request, authorizationHeader);
        request.Content = JsonContent.Create(new
        {
            storeLocationId,
            productId,
            stock,
        });

        return await _httpClient.SendAsync(request, cancellationToken);
    }

    private StoreProductDto MapToDto(StoreProductReadEntity product) => new()
    {
        ProductId = product.ProductId,
        Name = product.Name,
        Description = product.Description,
        Price = product.Price,
        ManufacturerId = product.ManufacturerId,
        ManufacturerName = product.ManufacturerName,
        CategoryId = product.CategoryId,
        CategoryName = product.CategoryName,
        ImageUrls = BuildImageUrls(product.ProductImages.Select(image => image.S3Key)),
        Store = new StoreProductStoreDto
        {
            StoreLocationId = product.StoreLocationId,
            Stock = product.Stock,
            DisplayName = product.StoreDisplayName,
            Address = product.StoreAddress,
        },
    };

    private List<string> BuildImageUrls(IEnumerable<string> s3Keys)
    {
        string baseUrl = _s3Configuration.ServiceUrl.TrimEnd('/');
        if (baseUrl.Contains("localstack", StringComparison.OrdinalIgnoreCase))
        {
            baseUrl = baseUrl.Replace("localstack", "localhost", StringComparison.OrdinalIgnoreCase);
        }

        return s3Keys
            .Where(key => !string.IsNullOrWhiteSpace(key))
            .Select(key => $"{baseUrl}/{_s3Configuration.BucketName}/{key.Trim()}")
            .ToList();
    }
}
