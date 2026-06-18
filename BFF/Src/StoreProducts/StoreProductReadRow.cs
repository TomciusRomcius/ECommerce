using BFF.ReadDb.Entities;

namespace BFF.StoreProducts;

public sealed class StoreProductReadRow
{
    public required StoreProductReadEntity StoreProduct { get; init; }

    public ProductEntity? Product { get; init; }

    public CategoryEntity? Category { get; init; }

    public ManufacturerEntity? Manufacturer { get; init; }
}
