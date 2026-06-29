using System.Text.Json;

namespace BFF.Products;

internal sealed class ProductWithImageKeysDto
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

public sealed class ProductWithImageUrlsDto
{
    public int ProductId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int ManufacturerId { get; set; }

    public int CategoryId { get; set; }

    public JsonElement? Manufacturer { get; set; }

    public JsonElement? Category { get; set; }

    public IReadOnlyList<string> ImageUrls { get; set; } = [];
}
