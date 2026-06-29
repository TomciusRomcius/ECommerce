using System.ComponentModel.DataAnnotations;

namespace StoreService.Presentation.Controllers.AvailableProducts.dtos;

public class GetProductsFromStoreDto
{
    [Required]
    public int StoreLocationId { get; set; }

    public int PageNumber { get; set; } = 0;

    public int PageSize { get; set; } = 20;
}
