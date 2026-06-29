namespace StoreService.Domain.Models;

public class DetailedProductModel
{
    public DetailedProductModel(
        int productId,
        string name,
        string description,
        decimal price,
        int manufacturerId,
        int categoryId,
        int stock)
    {
        Stock = stock;
    }

    public int ProductId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int ManufacturerId { get; set; }
    public int CategoryId { get; set; }
    public int Stock { get; set; }
}