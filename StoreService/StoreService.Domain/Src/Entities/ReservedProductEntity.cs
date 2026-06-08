namespace StoreService.Domain.Entities;

public class ReservedProductEntity
{
    public ReservedProductEntity(int storeLocationId, int productId, int stock)
    {
        StoreLocationId = storeLocationId;
        ProductId = productId;
        Stock = stock;
    }

    public int OrderId { get; set; }
    public int StoreLocationId { get; set; }
    public int ProductId { get; set; }
    public int Stock { get; set; }
    public StoreLocationEntity StoreLocation { get; set; } = null!;
}