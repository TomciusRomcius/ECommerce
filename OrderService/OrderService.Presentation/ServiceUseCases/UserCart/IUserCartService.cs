using OrderService.Presentation.ServiceUseCases.Utils;

namespace OrderService.Presentation.ServiceUseCases.UserCart;

public interface IUserCartService
{
    Task<Result<IEnumerable<CartProductModel>>> GetUserCartProductModelsAsync(Guid userId);
}
