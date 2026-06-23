using OrderService.Application.UseCases.UserCart;

namespace OrderService.Application.UseCases.OrderFlow;

public interface IOrderPriceCalculator
{
    decimal CalculatePrice(IEnumerable<CartProductModel> products);
}
