using OrderService.Application.UseCases.ProductDescription;

namespace OrderService.Application.UseCases.UserCart;

public class StoreProductModel
{
    public int Quantity { get; set; }

    public int Stock { get; set; }

    public required ProductModel Product { get; set; }

    public required StoreLocationModel StoreLocation { get; set; }
}
