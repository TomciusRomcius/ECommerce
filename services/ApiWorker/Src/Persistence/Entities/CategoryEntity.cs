using System.ComponentModel.DataAnnotations;

namespace ApiWorker.Persistence.Entities;

public sealed class CategoryEntity
{
    public CategoryEntity(string name)
    {
        Name = name;
    }

    public CategoryEntity(int categoryId, string name)
    {
        CategoryId = categoryId;
        Name = name;
    }

    [Key]
    public int CategoryId { get; set; }

    [Length(2, 50, ErrorMessage = "Category name length must be between 2 and 50 characters long")]
    public string Name { get; set; } = string.Empty;
}
