using AutoMapper;
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
    IMapper mapper,
    ILogger<StoreProductsService> logger) : IStoreProductsService
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient();

    public async Task<Page<StoreProductDto>> GetProductsAsync(
        int? storeLocationId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        IQueryable<StoreProductReadEntity> storeProductsQuery = readDbContext.StoreProducts.AsNoTracking();

        if (storeLocationId is not null)
        {
            storeProductsQuery = storeProductsQuery
                .Where(product => product.StoreLocationId == storeLocationId.Value);
        }

        storeProductsQuery = storeProductsQuery
            .OrderBy(storeProduct => storeProduct.StoreLocationId);
        int totalCount = await storeProductsQuery.CountAsync(cancellationToken);

        storeProductsQuery = storeProductsQuery
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);

        List<StoreProductDto> rows = await QueryJoinedProductsAsync(
            storeProductsQuery,
            cancellationToken);

        int fetchedCount = (pageNumber * pageSize) + rows.Count;
        return new Page<StoreProductDto>
        {
            Data = rows,
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
        IQueryable<StoreProductReadEntity> storeProductQuery = readDbContext.StoreProducts
            .AsNoTracking()
            .Where(storeProduct => storeProduct.ProductId == productId);

        List<StoreProductDto> rows = await QueryJoinedProductsAsync(
            storeProductQuery,
            cancellationToken);

        return rows.FirstOrDefault();
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

    private async Task<List<StoreProductDto>> QueryJoinedProductsAsync(
        IQueryable<StoreProductReadEntity> query,
        CancellationToken cancellationToken)
    {
        var products = await (from storeProduct in query
            join product in readDbContext.Products.AsNoTracking()
                on storeProduct.ProductId equals product.ProductId
            join category in readDbContext.Categories.AsNoTracking()
                on product.CategoryId equals category.CategoryId
            join manufacturer in readDbContext.Manufacturers.AsNoTracking()
                on product.ManufacturerId equals manufacturer.ManufacturerId
            join storeLocation in readDbContext.StoreLocations.AsNoTracking()
                on storeProduct.StoreLocationId equals storeLocation.StoreLocationId into storeLocations
            from storeLocation in storeLocations.DefaultIfEmpty()
            join image in readDbContext.ProductImages.AsNoTracking()
                on product.ProductId equals image.ProductId
            group new { storeProduct, product, storeLocation, manufacturer, category, image } by storeProduct.ProductId into g
            select new StoreProductReadRow
            {
                StoreProduct = g.First().storeProduct,
                Product = g.First().product,
                StoreLocation = g.First().storeLocation,
                Manufacturer = g.First().manufacturer,
                Category = g.First().category,
                Images = g.Select(x => x.image).ToList(),
            }
            ).ToListAsync();

            logger.LogDebug("Fetched {Count} products with joined data from ReadDB. Data: {@Products}", products.Count, products);
            
        return products.Select(product => mapper.Map<StoreProductDto>(product)).ToList();
    }
}
