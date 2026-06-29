using System.ComponentModel.DataAnnotations;

namespace UserService.Presentation.Controllers.Cart.dtos;

public class RequestAddItemDto
{
    [Required] public int ProductId { get; set; }

    [Required] public int StoreLocationId { get; set; }

    public int Quantity { get; set; } = 1;
}