using System.ComponentModel.DataAnnotations;

namespace BFF.Categories;

public class CreateCategoryRequest
{
    [Required(AllowEmptyStrings = false)]
    public required string Name { get; set; }
}
