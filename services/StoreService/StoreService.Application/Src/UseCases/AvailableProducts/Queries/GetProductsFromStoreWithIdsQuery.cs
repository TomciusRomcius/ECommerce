using MediatR;

namespace StoreService.Application.UseCases.AvailableProducts.Queries;

public record GetProductsFromStoreWithIdsQuery(List<int> ProductIds)
    : IRequest<List<ProductStoreLocationDetails>>;

public class ProductStoreLocationDetails
{
    public int ProductId { get; set; }
    public int StoreLocationId { get; set; }
    public int Stock { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}
