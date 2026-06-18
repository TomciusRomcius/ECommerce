using AutoMapper;
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
    IS3ImageUrlBuilder s3ImageUrlBuilder,
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

        List<StoreProductReadRow> rows = await FetchPaginatedJoinedRowsAsync(
            storeProductsQuery,
            cancellationToken);

        List<StoreProductDto> data = await MapRowsToDtosAsync(rows, cancellationToken);

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
        IQueryable<StoreProductReadEntity> storeProductQuery = readDbContext.StoreProducts
            .AsNoTracking()
            .Where(storeProduct => storeProduct.ProductId == productId);

        List<StoreProductReadRow> rows = await FetchPaginatedJoinedRowsAsync(
            storeProductQuery,
            cancellationToken);

        StoreProductReadRow? row = rows.FirstOrDefault();
        if (row is null)
        {
            return null;
        }

        List<StoreProductDto> dtos = await MapRowsToDtosAsync([row], cancellationToken);
        return dtos.FirstOrDefault();
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

    private async Task<List<StoreProductReadRow>> FetchPaginatedJoinedRowsAsync(
        IQueryable<StoreProductReadEntity> query,
        CancellationToken cancellationToken)
    {
        List<StoreProductReadRow> products = await (
            from storeProduct in query
            join product in readDbContext.Products.AsNoTracking()
                on storeProduct.ProductId equals product.ProductId
            join category in readDbContext.Categories.AsNoTracking()
                on product.CategoryId equals category.CategoryId
            join manufacturer in readDbContext.Manufacturers.AsNoTracking()
                on product.ManufacturerId equals manufacturer.ManufacturerId
            join storeLocation in readDbContext.StoreLocations.AsNoTracking()
                on storeProduct.StoreLocationId equals storeLocation.StoreLocationId into storeLocations
            from storeLocation in storeLocations.DefaultIfEmpty()
            select new StoreProductReadRow
            {
                StoreProduct = storeProduct,
                Product = product,
                Category = category,
                Manufacturer = manufacturer,
                StoreLocation = storeLocation,
            }
        ).ToListAsync(cancellationToken);

        return products;
    }

    private async Task<List<StoreProductDto>> MapRowsToDtosAsync(
        IReadOnlyList<StoreProductReadRow> rows,
        CancellationToken cancellationToken)
    {
        if (rows.Count == 0)
        {
            return [];
        }

        List<int> productIds = rows.Select(row => row.StoreProduct.ProductId).Distinct().ToList();

        List<ProductImageReadEntity> images = await readDbContext.ProductImages
            .AsNoTracking()
            .Where(image => productIds.Contains(image.ProductId))
            .ToListAsync(cancellationToken);

        Dictionary<int, List<ProductImageReadEntity>> imagesByProductId = images
            .GroupBy(image => image.ProductId)
            .ToDictionary(group => group.Key, group => group.ToList());

        List<StoreProductDto> dtos = mapper.Map<List<StoreProductDto>>(rows);

        foreach (StoreProductDto dto in dtos)
        {
            if (!imagesByProductId.TryGetValue(dto.ProductId, out List<ProductImageReadEntity>? productImages))
            {
                continue;
            }

            dto.ImageUrls = s3ImageUrlBuilder
                .BuildUrls(productImages.Select(image => image.S3Key))
                .ToList();
        }

        return dtos;
    }
}
