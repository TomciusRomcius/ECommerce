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

        List<StoreProductDto> data = mapper.Map<List<StoreProductDto>>(rows);

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

        return product is null ? null : mapper.Map<StoreProductDto>(product);
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
