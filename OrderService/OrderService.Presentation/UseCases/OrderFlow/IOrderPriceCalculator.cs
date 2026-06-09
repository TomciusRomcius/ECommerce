using OrderService.Presentation.ServiceUseCases.UserCart;

namespace OrderService.Presentation.UseCases.OrderFlow;

public interface IOrderPriceCalculator
{
    decimal CalculatePrice(IEnumerable<CartProductModel> products);
}