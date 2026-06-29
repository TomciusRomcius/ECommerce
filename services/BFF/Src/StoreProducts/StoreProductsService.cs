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
        string orderBy,
        string orderType,
        CancellationToken cancellationToken = default)
    {
        IQueryable<StoreProductReadEntity> query = readDbContext.StoreProducts.AsNoTracking();

        if (storeLocationId is not null)
        {
            query = query
                .Where(product => product.StoreLocationId == storeLocationId.Value);
        }

        var joinedQuery = query
            .Join(readDbContext.Products.AsNoTracking(),
                storeProduct => storeProduct.ProductId,
                product => product.ProductId,
                (storeProduct, product) => new { storeProduct, product })
            .Join(readDbContext.Categories.AsNoTracking(),
                sp => sp.product.CategoryId,
                category => category.CategoryId,
                (sp, category) => new { sp.storeProduct, sp.product, category })
            .Join(readDbContext.Manufacturers.AsNoTracking(),
                sp => sp.product.ManufacturerId,
                manufacturer => manufacturer.ManufacturerId,
                (sp, manufacturer) => new { sp.storeProduct, sp.product, sp.category, manufacturer })
            .Join(readDbContext.StoreLocations.AsNoTracking(),
                sp => sp.storeProduct.StoreLocationId,
                storeLocation => storeLocation.StoreLocationId,
                (sp, storeLocation) => new { sp.storeProduct, sp.product, sp.category, sp.manufacturer, storeLocation });

        int totalCount = await query.CountAsync(cancellationToken);
        var orderedQuery = orderBy.ToLower() switch
        {
            "name" => orderType == OrderType.Ascending
                ? joinedQuery.OrderBy(row => row.product.Name)
                : joinedQuery.OrderByDescending(row => row.product.Name),
            "price" => orderType == OrderType.Ascending
                ? joinedQuery.OrderBy(row => row.product.Price)
                : joinedQuery.OrderByDescending(row => row.product.Price),
            "stock" => orderType == OrderType.Ascending
                ? joinedQuery.OrderBy(row => row.storeProduct.Stock)
                : joinedQuery.OrderByDescending(row => row.storeProduct.Stock),
            "storelocation" => orderType == OrderType.Ascending
                ? joinedQuery.OrderBy(row => row.storeLocation.DisplayName)
                : joinedQuery.OrderByDescending(row => row.storeLocation.DisplayName),
            _ => joinedQuery.OrderBy(row => row.product.ProductId),
        };

        joinedQuery = orderedQuery
            .ThenBy(x => x.product.ProductId)
            .ThenBy(x => x.storeProduct.StoreLocationId);

        var resultQuery = joinedQuery
            .Select(g => new StoreProductReadRow
            {
                StoreProduct = g.storeProduct,
                Product = g.product,
                StoreLocation = g.storeLocation,
                Manufacturer = g.manufacturer,
                Category = g.category,
                Images = readDbContext.ProductImages
                    .Where(image => image.ProductId == g.product.ProductId)
                    .ToList()
            });
        
        resultQuery = resultQuery
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);

        var fetchedProducts = await resultQuery.ToListAsync(cancellationToken);

        int fetchedCount = (pageNumber * pageSize) + fetchedProducts.Count;
        return new Page<StoreProductDto>
        {
            Data = fetchedProducts.Select(row => mapper.Map<StoreProductDto>(row)).ToList(),
            TotalCount = totalCount,
            HasNextPage = fetchedCount < totalCount,
            HasPrevPage = pageNumber > 1,
            PageSize = pageSize,
        };
    }

    public async Task<StoreProductDto?> GetProductByIdAsync(
        int productId,
        CancellationToken cancellationToken = default)
    {
        var query = readDbContext.StoreProducts
            .AsNoTracking()
            .Where(storeProduct => storeProduct.ProductId == productId)
            .Join(readDbContext.Products.AsNoTracking(),
                storeProduct => storeProduct.ProductId,
                product => product.ProductId,
                (storeProduct, product) => new { storeProduct, product })
            .Join(readDbContext.Categories.AsNoTracking(),
                sp => sp.product.CategoryId,
                category => category.CategoryId,
                (sp, category) => new { sp.storeProduct, sp.product, category })
            .Join(readDbContext.Manufacturers.AsNoTracking(),
                sp => sp.product.ManufacturerId,
                manufacturer => manufacturer.ManufacturerId,
                (sp, manufacturer) => new { sp.storeProduct, sp.product, sp.category, manufacturer })
            .Join(readDbContext.StoreLocations.AsNoTracking(),
                sp => sp.storeProduct.StoreLocationId,
                storeLocation => storeLocation.StoreLocationId,
                (sp, storeLocation) => new { sp.storeProduct, sp.product, sp.category, sp.manufacturer, storeLocation })
            .Select(g => new StoreProductReadRow
            {
                StoreProduct = g.storeProduct,
                Product = g.product,
                StoreLocation = g.storeLocation,
                Manufacturer = g.manufacturer,
                Category = g.category,
                Images = readDbContext.ProductImages
                    .Where(image => image.ProductId == g.product.ProductId)
                    .ToList()
            });

        var fetchedProducts = await query.FirstOrDefaultAsync(cancellationToken);

        return mapper.Map<StoreProductDto>(fetchedProducts);
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
}
