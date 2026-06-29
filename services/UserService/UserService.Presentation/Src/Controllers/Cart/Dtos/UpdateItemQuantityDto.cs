using System.ComponentModel.DataAnnotations;

namespace UserService.Presentation.Controllers.Cart.dtos;

public class UpdateItemQuantityDto
{
    [Required] public int ProductId { get; set; }

    public int Quantity { get; set; } = 1;
}