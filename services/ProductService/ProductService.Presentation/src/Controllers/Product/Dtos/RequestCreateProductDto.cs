using System.ComponentModel.DataAnnotations;

namespace ProductService.Presentation.Controllers.Product.Dtos;

public class RequestCreateProductDto
{
    [Required(AllowEmptyStrings = false, ErrorMessage = "Name cannot be empty")]
    public required string Name { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "Description cannot be empty")]
    public required string Description { get; set; }

    public required decimal Price { get; set; }
    public required int ManufacturerId { get; set; }
    public required int CategoryId { get; set; }

    public List<string> ImageKeys { get; set; } = [];

    /// <summary>
    /// When ImageKeys is empty, generates keys as {productId}_{order} for order 0..ImageCount-1.
    /// </summary>
    public int ImageCount { get; set; }
}