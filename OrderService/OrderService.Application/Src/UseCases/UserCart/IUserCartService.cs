using OrderService.Application.Utils;

namespace OrderService.Application.UseCases.UserCart;

public interface IUserCartService
{
    Task<Result<IEnumerable<CartProductModel>>> GetUserCartProductModelsAsync(Guid userId);
}
