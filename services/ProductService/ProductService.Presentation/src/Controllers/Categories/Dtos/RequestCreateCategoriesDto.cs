using System.ComponentModel.DataAnnotations;

namespace ProductService.Presentation.Controllers.Categories.Dtos;

public class RequestCreateCategoryDto
{
    [Required(AllowEmptyStrings = false, ErrorMessage = "Name cannot be empty")]
    public required string Name { get; set; }
}