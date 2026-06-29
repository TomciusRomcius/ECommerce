using BFF.Utils;
using ECommerceBackend.Utils.Pagination;

namespace BFF.Products;

public interface IProductService
{
    Task<Result<Page<ProductWithImageUrlsDto>>> GetProductsAsync(
        string searchText,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<Result<ProductWithImageUrlsDto>> GetProductAsync(
        int productId,
        CancellationToken cancellationToken = default);

    Task<HttpResponseMessage> CreateProductAsync(
        CreateProductRequest request,
        string? authorizationHeader,
        CancellationToken cancellationToken = default);
}
