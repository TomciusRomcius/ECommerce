namespace BFF.ReadDb.Entities;

public sealed class CartProductReadEntity
{
    public string UserId { get; set; } = string.Empty;
    public int StoreLocationId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}
