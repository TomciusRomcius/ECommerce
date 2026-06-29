using System.ComponentModel.DataAnnotations;

namespace StoreService.Presentation.Controllers.AvailableProducts.dtos;

public class AddProductToStoreDto
{
    [Required] public int StoreLocationId { get; set; }
    [Required] public int ProductId { get; set; }
    [Required] public int Stock { get; set; }
}
