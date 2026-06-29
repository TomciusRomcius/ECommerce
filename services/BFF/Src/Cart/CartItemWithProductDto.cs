using BFF.StoreProducts;

namespace BFF.Cart;

public class CartItemWithProductDto
{
    public string UserId { get; set; } = string.Empty;
    public int ProductId { get; set; }
    public int StoreLocationId { get; set; }
    public int Quantity { get; set; }
    public StoreProductDto? Product { get; set; }
}
