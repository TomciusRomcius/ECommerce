using OrderService.Application.Utils;

namespace OrderService.Application.UseCases.UserCart;

public interface IUserCartService
{
    Task<Result<IReadOnlyList<StoreProductModel>>> GetUserCartStoreProductsAsync(Guid userId);
}
