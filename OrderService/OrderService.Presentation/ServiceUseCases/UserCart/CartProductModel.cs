namespace OrderService.Presentation.ServiceUseCases.UserCart;

public class CartProductModel : CartProductMinimalModel
{
    public Guid UserId { get; set; }
    public decimal Price { get; set; }
}