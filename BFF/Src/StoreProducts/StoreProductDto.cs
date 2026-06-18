using BFF.StoreLocations;

namespace BFF.StoreProducts;

public class StoreProductDto
{
    public int ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int ManufacturerId { get; set; }
    public int Stock { get; set; }
    public string ManufacturerName { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public StoreLocationDto? Store { get; set; }
    public List<string> ImageUrls { get; set; } = [];
}
