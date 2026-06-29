using System.ComponentModel.DataAnnotations;

namespace BFF.ReadDb.Entities;

public sealed class CategoryEntity
{
    [Key]
    public int CategoryId { get; set; }

    public string Name { get; set; } = string.Empty;
}
