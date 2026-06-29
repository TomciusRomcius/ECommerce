namespace ProductService.Domain.Models;

public class UpdateCategoryModel
{
    public UpdateCategoryModel(int categoryId, string? name)
    {
        CategoryId = categoryId;
        Name = name;
    }

    public int CategoryId { get; set; }
    public string? Name { get; set; }
}