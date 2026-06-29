using System.ComponentModel.DataAnnotations;

namespace BFF.Manufacturers;

public class CreateManufacturerRequest
{
    [Required(AllowEmptyStrings = false)]
    public required string Name { get; set; }
}
