namespace StoreService.Domain.Entities;

public class ReservedProductEntity
{
    public ReservedProductEntity(Guid orderId, int storeLocationId, int productId, int stock)
    {
        OrderId = orderId;
        StoreLocationId = storeLocationId;
        ProductId = productId;
        Stock = stock;
    }

    public Guid OrderId { get; set; }
    public int StoreLocationId { get; set; }
    public int ProductId { get; set; }
    public int Stock { get; set; }
}
