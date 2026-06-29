namespace OrderService.Application.UseCases.UserCart;

public class StoreLocationModel
{
    public int StoreLocationId { get; set; }

    public string DisplayName { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;
}
