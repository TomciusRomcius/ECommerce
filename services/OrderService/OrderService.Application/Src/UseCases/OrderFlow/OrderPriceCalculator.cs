using OrderService.Application.UseCases.UserCart;

namespace OrderService.Application.UseCases.OrderFlow;

public class OrderPriceCalculator : IOrderPriceCalculator
{
    public decimal CalculatePrice(IEnumerable<CartProductModel> products)
        => products.Sum(product => product.Price);
}
