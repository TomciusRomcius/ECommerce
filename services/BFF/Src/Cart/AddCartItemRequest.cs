using System.ComponentModel.DataAnnotations;

namespace BFF.Cart;

public class AddCartItemRequest
{
    [Required]
    public int ProductId { get; set; }
    [Required]
    public int StoreLocationId { get; set; }
    public int Quantity { get; set; } = 1;
}
