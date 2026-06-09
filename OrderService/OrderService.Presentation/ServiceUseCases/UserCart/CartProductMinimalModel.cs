namespace OrderService.Presentation.ServiceUseCases.UserCart;

// CartProductModel without price for one unit
public class CartProductMinimalModel
{
    public Guid UserId { get; set; }
    public int StoreLocationId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}