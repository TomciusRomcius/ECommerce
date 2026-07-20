namespace ProductService.Domain.Models;

public class UpdateProductModel
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public int? ManufacturerId { get; set; }
    public int? CategoryId { get; set; }
}
