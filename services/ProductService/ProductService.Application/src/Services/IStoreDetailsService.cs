namespace ProductService.Application.Services;

public interface IStoreDetailsService
{
    Task<IReadOnlyDictionary<int, ProductStoreDetails>> GetStoreDetailsByProductIdsAsync(
        IEnumerable<int> productIds,
        CancellationToken cancellationToken = default);
}

public class ProductStoreDetails
{
    public int StoreLocationId { get; set; }

    public int Stock { get; set; }

    public string DisplayName { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;
}
