using System.ComponentModel.DataAnnotations;

namespace UserService.Presentation.Controllers.Cart;

public class GetItemsDto
{
    [Required]
    public string UserId { get; set; }
}