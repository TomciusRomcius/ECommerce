using System.ComponentModel.DataAnnotations;

namespace StoreService.Presentation.Controllers.AvailableProducts.dtos;

public class ReserveProductsDto
{
    [Required]
    public Guid OrderId { get; set; }

    [Required]
    public List<ReserveProductItemDto> Products { get; set; } = [];
}

public class ReserveProductItemDto
{
    [Required]
    public int StoreLocationId { get; set; }

    [Required]
    public int ProductId { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int Stock { get; set; }
}
