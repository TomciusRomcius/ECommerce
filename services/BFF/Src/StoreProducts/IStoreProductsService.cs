using ECommerceBackend.Utils.Pagination;

namespace BFF.StoreProducts;

public interface IStoreProductsService
{
    Task<Page<StoreProductDto>> GetProductsAsync(
        int? storeLocationId,
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderType,
        CancellationToken cancellationToken = default);

    Task<StoreProductDto?> GetProductByIdAsync(int productId, CancellationToken cancellationToken = default);

    Task<HttpResponseMessage> UpdateProductStockAsync(
        int storeLocationId,
        int productId,
        int stock,
        string? authorizationHeader,
        CancellationToken cancellationToken = default);

    Task<HttpResponseMessage> AddProductToStoreAsync(
        int storeLocationId,
        int productId,
        int stock,
        string? authorizationHeader,
        CancellationToken cancellationToken = default);
}
