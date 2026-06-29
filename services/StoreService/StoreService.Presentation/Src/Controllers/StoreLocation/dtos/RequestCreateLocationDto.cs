using System.ComponentModel.DataAnnotations;

namespace StoreService.Presentation.Controllers.StoreLocation.dtos;

public class RequestCreateLocationDto
{
    [Required] public required string DisplayName { get; set; }

    [Required] public required string Address { get; set; }
}