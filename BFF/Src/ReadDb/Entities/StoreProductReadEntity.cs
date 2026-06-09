namespace BFF.ReadDb.Entities;

public sealed class StoreProductReadEntity
{
    public int StoreLocationId { get; set; }

    public int ProductId { get; set; }

    public int Stock { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int ManufacturerId { get; set; }

    public int CategoryId { get; set; }

    public string ManufacturerName { get; set; } = string.Empty;

    public string CategoryName { get; set; } = string.Empty;

    public string StoreDisplayName { get; set; } = string.Empty;

    public string StoreAddress { get; set; } = string.Empty;

    public ICollection<ProductImageReadEntity> ProductImages { get; set; } = [];
}
