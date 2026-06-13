namespace BFF.ReadDb.Entities;

public sealed class StoreProductReadEntity
{
    public int StoreLocationId { get; set; }

    public int ProductId { get; set; }

    public int Stock { get; set; }

    public string StoreDisplayName { get; set; } = string.Empty;

    public string StoreAddress { get; set; } = string.Empty;

    public ProductEntity Product { get; set; } = null!;
}
