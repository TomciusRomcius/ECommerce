using System.ComponentModel.DataAnnotations;

namespace StoreService.Domain.Entities;

public class ProductStoreLocationEntity
{
    public ProductStoreLocationEntity(int storeLocationId, int productId, int stock)
    {
        StoreLocationId = storeLocationId;
        ProductId = productId;
        Stock = stock;
    }

    public int StoreLocationId { get; set; }

    public int ProductId { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be lower than 0!")]
    public int Stock { get; set; }
}