namespace OrderService.Application.UseCases.ProductDescription;

public class ProductModel
{
    public int ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int ManufacturerId { get; set; }
    public int CategoryId { get; set; }
}
