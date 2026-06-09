using System.Net.Http.Json;
using System.Text.Json;
using BFF.Configuration;
using BFF.Utils;
using ECommerceBackend.Utils.Microservices;
using ECommerceBackend.Utils.Pagination;
using Microsoft.Extensions.Options;

namespace BFF.StoreProducts;

public class StoreProductsService(
    HttpClient httpClient,
    IOptions<MicroserviceHosts> hosts,
    IS3ImageUrlBuilder s3ImageUrlBuilder,
    ILogger<StoreProductsService> logger) : IStoreProductsService
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public async Task<Page<StoreProductDto>> GetProductsAsync(
        int? storeLocationId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        Page<ProductFromServiceDto> productPage = await FetchProductPageAsync(pageNumber, pageSize, cancellationToken);

        if (productPage.Data.Count == 0)
        {
            return new Page<StoreProductDto>
            {
                Data = [],
                TotalCount = productPage.TotalCount,
                HasNextPage = productPage.HasNextPage,
                HasPrevPage = productPage.HasPrevPage,
                PageSize = pageSize
            };
        }

        IReadOnlyDictionary<int, ProductStoreLocationDto> storeDetailsByProductId =
            await FetchProductStoreDetailsByProductIdsAsync(
                productPage.Data.Select(product => product.ProductId),
                storeLocationId,
                cancellationToken);

        List<StoreProductDto> merged = StoreProductMerger.Merge(
            productPage.Data,
            storeDetailsByProductId,
            product => s3ImageUrlBuilder.BuildUrls(product.ImageKeys),
            storeLocationId);

        return new Page<StoreProductDto>
        {
            Data = merged,
            TotalCount = productPage.TotalCount,
            HasNextPage = productPage.HasNextPage,
            HasPrevPage = productPage.HasPrevPage,
            PageSize = pageSize,
        };
    }

    public async Task<StoreProductDto?> GetProductByIdAsync(int productId, CancellationToken cancellationToken = default)
    {
        List<ProductFromServiceDto> products = await FetchProductsByIdsAsync([productId], cancellationToken);
        ProductFromServiceDto? product = products.FirstOrDefault();

        if (product is null)
        {
            return null;
        }

        IReadOnlyDictionary<int, ProductStoreLocationDto> storeDetailsByProductId =
            await FetchProductStoreDetailsByProductIdsAsync([productId], storeLocationId: null, cancellationToken);

        storeDetailsByProductId.TryGetValue(productId, out ProductStoreLocationDto? storeDetails);
        IReadOnlyList<string> imageUrls = s3ImageUrlBuilder.BuildUrls(product.ImageKeys);
        return StoreProductMerger.ToStoreProductDto(product, storeDetails, imageUrls);
    }

    private async Task<Page<ProductFromServiceDto>> FetchProductPageAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = new QueryString()
            .Add("pageNumber", pageNumber.ToString())
            .Add("pageSize", pageSize.ToString());

        string productsUrl = $"{hosts.Value.ProductServiceUrl}/product{query}";
        logger.LogDebug("Fetching products from {Url}", productsUrl);

        using HttpResponseMessage response = await httpClient.GetAsync(productsUrl, cancellationToken);
        response.EnsureSuccessStatusCode();

        Page<ProductFromServiceDto>? page =
            await response.Content.ReadFromJsonAsync<Page<ProductFromServiceDto>>(JsonOptions, cancellationToken);

        return page ?? new Page<ProductFromServiceDto>
        {
            Data = [],
            TotalCount = 0,
            HasNextPage = false,
            HasPrevPage = false,
        };
    }

    private async Task<List<ProductFromServiceDto>> FetchProductsByIdsAsync(
        IEnumerable<int> productIds,
        CancellationToken cancellationToken)
    {
        List<int> ids = productIds.Distinct().ToList();
        if (ids.Count == 0)
        {
            return [];
        }

        var query = new QueryString();
        foreach (int id in ids)
        {
            query = query.Add("ids", id.ToString());
        }

        string productsUrl = $"{hosts.Value.ProductServiceUrl}/product/by-ids{query}";
        logger.LogDebug("Fetching products by ids from {Url}", productsUrl);

        using HttpResponseMessage response = await httpClient.GetAsync(productsUrl, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<List<ProductFromServiceDto>>(JsonOptions, cancellationToken)
            ?? [];
    }

    private async Task<IReadOnlyDictionary<int, ProductStoreLocationDto>> FetchProductStoreDetailsByProductIdsAsync(
        IEnumerable<int> productIds,
        int? storeLocationId,
        CancellationToken cancellationToken)
    {
        List<int> ids = productIds.Distinct().ToList();
        if (ids.Count == 0)
        {
            return new Dictionary<int, ProductStoreLocationDto>();
        }

        var query = new QueryString();
        foreach (int id in ids)
        {
            query = query.Add("ids", id.ToString());
        }

        string storeUrl = $"{hosts.Value.StoreServiceUrl}/availableproducts/by-product-ids{query}";
        logger.LogDebug("Fetching store details from {Url}", storeUrl);

        using HttpResponseMessage response = await httpClient.GetAsync(storeUrl, cancellationToken);
        response.EnsureSuccessStatusCode();

        List<ProductStoreLocationDto>? storeDetails =
            await response.Content.ReadFromJsonAsync<List<ProductStoreLocationDto>>(JsonOptions, cancellationToken);

        return StoreProductMerger.IndexStoreDetails(storeDetails ?? [], storeLocationId);
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

        return await httpClient.SendAsync(request, cancellationToken);
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

        return await httpClient.SendAsync(request, cancellationToken);
    }
}
