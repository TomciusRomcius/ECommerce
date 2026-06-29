using System.ComponentModel.DataAnnotations;

namespace ApiWorker.Persistence.Entities;

public sealed class ManufacturerEntity
{
    public ManufacturerEntity(string name)
    {
        Name = name;
    }

    public ManufacturerEntity(int manufacturerId, string name)
    {
        ManufacturerId = manufacturerId;
        Name = name;
    }

    [Key]
    public int ManufacturerId { get; set; }

    public string Name { get; set; } = string.Empty;
}
