using System.Text.Json;

namespace BFF.StoreProducts;

internal sealed class ProductFromServiceDto
{
    public int ProductId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int ManufacturerId { get; set; }

    public int CategoryId { get; set; }

    public JsonElement? Manufacturer { get; set; }

    public JsonElement? Category { get; set; }

    public List<string> ImageKeys { get; set; } = [];
}

internal sealed class ProductStoreLocationDto
{
    public int ProductId { get; set; }

    public int StoreLocationId { get; set; }

    public int Stock { get; set; }

    public string DisplayName { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;
}
