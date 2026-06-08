namespace StoreService.Application.UseCases.AvailableProducts;

internal sealed class ProductStoreLocationQueryRow
{
    public int ProductId { get; set; }
    public int StoreLocationId { get; set; }
    public int Stock { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}
