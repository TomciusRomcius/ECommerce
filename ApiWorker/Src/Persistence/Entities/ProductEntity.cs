using System.ComponentModel.DataAnnotations;

namespace ApiWorker.Persistence.Entities;

public sealed class ProductEntity
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

    [Key]
    public int ProductId { get; set; }

    [MinLength(5, ErrorMessage = "Invalid ProductId")]
    public string Name { get; set; } = string.Empty;

    [MinLength(5, ErrorMessage = "Invalid ProductId")]
    public string Description { get; set; } = string.Empty;

    [Range(0.1, double.MaxValue, ErrorMessage = "Invalid ProductId")]
    public decimal Price { get; set; }

    public int ManufacturerId { get; set; }

    public int CategoryId { get; set; }
}
