using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace BFF.Products;

public class CreateProductDetails
{
}

public class CreateProductRequest
{
    public List<IFormFile> Files { get; set; } = [];
    [Required(AllowEmptyStrings = false)]
    public required string Name { get; set; }
    [Required(AllowEmptyStrings = false)]
    public required string Description { get; set; }
    public required decimal Price { get; set; }
    public required int ManufacturerId { get; set; }
    public required int CategoryId { get; set; }
}
