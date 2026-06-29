using System.ComponentModel.DataAnnotations;

namespace UserService.Domain.Entities;

public class CartProductEntity
{
    public CartProductEntity(string userId, int productId, int storeLocationId, int quantity)
    {
        UserId = userId;
        ProductId = productId;
        StoreLocationId = storeLocationId;
        Quantity = quantity;
    }

    public string UserId { get; set; }
    public int ProductId { get; set; }
    public int StoreLocationId { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than one!")]
    public int Quantity { get; set; }
}