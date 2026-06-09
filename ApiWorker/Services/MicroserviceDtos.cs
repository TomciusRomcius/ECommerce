namespace ApiWorker.Services;

internal sealed class StoreLocationDto
{
    public int StoreLocationId { get; set; }

    public string DisplayName { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;
}

internal sealed class ProductDto
{
    public int ProductId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int ManufacturerId { get; set; }

    public int CategoryId { get; set; }

    public NamedEntityDto? Manufacturer { get; set; }

    public NamedEntityDto? Category { get; set; }

    public List<string> ImageKeys { get; set; } = [];
}

internal sealed class NamedEntityDto
{
    public string Name { get; set; } = string.Empty;
}
