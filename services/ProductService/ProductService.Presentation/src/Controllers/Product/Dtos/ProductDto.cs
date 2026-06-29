using ProductService.Domain.Entities;

namespace ProductService.Presentation.Controllers.Product.Dtos;

public class ProductDto
{
    public int ProductId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int ManufacturerId { get; set; }

    public int CategoryId { get; set; }

    public ManufacturerEntity? Manufacturer { get; set; }

    public CategoryEntity? Category { get; set; }

    public List<string> ImageKeys { get; set; } = [];
}
