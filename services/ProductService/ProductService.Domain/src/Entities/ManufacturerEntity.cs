using System.ComponentModel.DataAnnotations;

namespace ProductService.Domain.Entities;

public class ManufacturerEntity
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

    [Key] public int ManufacturerId { get; set; }

    public string Name { get; set; }
}