using System.ComponentModel.DataAnnotations;

namespace BFF.Products;

public class UpdateProductRequest
{
    [Required(AllowEmptyStrings = false)]
    public required string Name { get; set; }

    [Required(AllowEmptyStrings = false)]
    public required string Description { get; set; }

    public required decimal Price { get; set; }
    public required int ManufacturerId { get; set; }
    public required int CategoryId { get; set; }
}
