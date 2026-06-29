using System.ComponentModel.DataAnnotations;

namespace BFF.StoreProducts;

public class UpdateProductStockRequest
{
    [Required] public int StoreLocationId { get; set; }

    [Required] public int ProductId { get; set; }

    [Required] public int Stock { get; set; }
}
