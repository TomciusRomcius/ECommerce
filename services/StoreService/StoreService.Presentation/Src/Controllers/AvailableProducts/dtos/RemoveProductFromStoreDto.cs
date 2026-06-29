using System.ComponentModel.DataAnnotations;

namespace StoreService.Presentation.Controllers.AvailableProducts.dtos;

public class RemoveProductFromStoreDto
{
    [Required] public int StoreLocationId { get; set; }

    [Required] public int ProductId { get; set; }
}
