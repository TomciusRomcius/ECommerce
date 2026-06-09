using OrderService.Presentation.ServiceUseCases.UserCart;

namespace OrderService.Presentation.UseCases.OrderFlow;

public class OrderPriceCalculator : IOrderPriceCalculator
{
    public decimal CalculatePrice(IEnumerable<CartProductModel> products)
        => products.Sum(product => product.Price);
}