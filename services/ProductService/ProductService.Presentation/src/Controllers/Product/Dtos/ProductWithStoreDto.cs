using ProductService.Domain.Entities;

namespace ProductService.Presentation.Controllers.Product.Dtos;

public class ProductWithStoreDto
{
    public int ProductId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int ManufacturerId { get; set; }

    public int CategoryId { get; set; }

    public ManufacturerEntity? Manufacturer { get; set; }

    public CategoryEntity? Category { get; set; }

    public StoreDetailsDto? Store { get; set; }
}

public class StoreDetailsDto
{
    public int StoreLocationId { get; set; }

    public int Stock { get; set; }

    public string DisplayName { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;
}
