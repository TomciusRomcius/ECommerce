using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductService.Domain.Entities;

public class ProductEntity
{
    public ProductEntity(string name, string description, decimal price, int manufacturerId, int categoryId)
    {
        Name = name;
        Description = description;
        Price = price;
        ManufacturerId = manufacturerId;
        CategoryId = categoryId;
    }

    public ProductEntity(int productId, string name, string description, decimal price, int manufacturerId,
        int categoryId)
    {
        ProductId = productId;
        Name = name;
        Description = description;
        Price = price;
        ManufacturerId = manufacturerId;
        CategoryId = categoryId;
    }

    [Key] public int ProductId { get; set; }

    [MinLength(5, ErrorMessage = "Invalid ProductId")]
    public string Name { get; set; }

    [MinLength(5, ErrorMessage = "Invalid ProductId")]
    public string Description { get; set; }

    [Range(0.1, double.MaxValue, ErrorMessage = "Invalid ProductId")]
    public decimal Price { get; set; }

    [ForeignKey(nameof(Manufacturer))] public int ManufacturerId { get; set; }

    [ForeignKey(nameof(Category))] public int CategoryId { get; set; }

    public ManufacturerEntity? Manufacturer { get; set; }
    public CategoryEntity? Category { get; set; }
    public ICollection<ProductImageEntity> Images { get; set; } = [];
}