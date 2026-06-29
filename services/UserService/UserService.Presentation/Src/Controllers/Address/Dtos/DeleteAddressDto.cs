using System.ComponentModel.DataAnnotations;

namespace UserService.Presentation.Controllers.Address.dtos;

public class DeleteAddressDto
{
    [Required] public bool IsShipping { get; set; }
}