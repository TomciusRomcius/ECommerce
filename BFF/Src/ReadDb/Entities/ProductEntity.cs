using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BFF.ReadDb.Entities;

public sealed class ProductEntity
{
    [Key]
    public int ProductId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; }

    [ForeignKey(nameof(Manufacturer))]
    public int ManufacturerId { get; set; }

    [ForeignKey(nameof(Category))]
    public int CategoryId { get; set; }

    public ManufacturerEntity? Manufacturer { get; set; }
    public CategoryEntity? Category { get; set; }
    public ICollection<ProductImageReadEntity> Images { get; set; } = [];
}
