namespace ProductService.Domain.Models;

public class UpdateManufacturerModel
{
    public UpdateManufacturerModel(int manufacturerId, string? name)
    {
        ManufacturerId = manufacturerId;
        Name = name;
    }

    public int ManufacturerId { get; set; }
    public string? Name { get; set; }
}