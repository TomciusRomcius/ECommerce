namespace OrderService.Application.UseCases.UserCart;

public class CartProductMinimalModel
{
    public Guid UserId { get; set; }
    public int StoreLocationId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}
