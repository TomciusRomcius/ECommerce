using OrderService.Application.UseCases.UserCart;
using OrderService.Application.Utils;

namespace OrderService.Application.UseCases.ProductReservation;

public interface IProductReservationService
{
    Task<ResultError?> ReserveProductsAsync(
        Guid orderId,
        IEnumerable<StoreProductModel> cartProducts,
        CancellationToken cancellationToken = default);
}
