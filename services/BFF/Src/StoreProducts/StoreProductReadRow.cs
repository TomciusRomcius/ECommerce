using BFF.ReadDb.Entities;

namespace BFF.StoreProducts;

public sealed class StoreProductReadRow
{
    public required StoreProductReadEntity StoreProduct { get; init; }
    
    public required ProductEntity? Product { get; init; }

    public required CategoryEntity? Category { get; init; }

    public required ManufacturerEntity? Manufacturer { get; init; }

    public required StoreLocationEntity? StoreLocation { get; init; }

    public required IEnumerable<ProductImageReadEntity> Images { get; init; } = [];
}
