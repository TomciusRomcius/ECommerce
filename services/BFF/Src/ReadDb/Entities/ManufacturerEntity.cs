using System.ComponentModel.DataAnnotations;

namespace BFF.ReadDb.Entities;

public sealed class ManufacturerEntity
{
    [Key]
    public int ManufacturerId { get; set; }

    public string Name { get; set; } = string.Empty;
}
