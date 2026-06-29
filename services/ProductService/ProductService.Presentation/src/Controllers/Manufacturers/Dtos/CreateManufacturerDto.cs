using System.ComponentModel.DataAnnotations;

namespace ProductService.Presentation.Controllers.Manufacturers.Dtos;

public class RequestCreateManufacturerDto
{
    [Required(AllowEmptyStrings = false, ErrorMessage = "You must provide a name")]
    public required string Name { get; set; }
}

public class ResponseCreateManufacturerDto
{
    public required int ManufacturerId { get; set; }
}