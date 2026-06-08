using System.Net.Http.Json;
using ECommerceBackend.Utils.Microservices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ProductService.Application.Services;

public class StoreDetailsService(
    HttpClient httpClient,
    IOptions<MicroserviceHosts> hosts,
    ILogger<StoreDetailsService> logger) : IStoreDetailsService
{
    public async Task<IReadOnlyDictionary<int, ProductStoreDetails>> GetStoreDetailsByProductIdsAsync(
        IEnumerable<int> productIds,
        CancellationToken cancellationToken = default)
    {
        List<int> ids = productIds.Distinct().ToList();
        if (ids.Count == 0)
        {
            return new Dictionary<int, ProductStoreDetails>();
        }

        string query = string.Join(
            "&",
            ids.Select(id => $"ids={Uri.EscapeDataString(id.ToString())}"));
        string url = $"{hosts.Value.StoreServiceUrl}/availableproducts/by-product-ids?{query}";
        logger.LogDebug("Fetching store details from {Url}", url);

        using HttpResponseMessage response = await httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();

        List<StoreServiceProductStoreLocationDto>? storeLocations =
            await response.Content.ReadFromJsonAsync<List<StoreServiceProductStoreLocationDto>>(cancellationToken);

        return (storeLocations ?? [])
            .GroupBy(location => location.ProductId)
            .ToDictionary(
                group => group.Key,
                group =>
                {
                    StoreServiceProductStoreLocationDto location = group.First();
                    return new ProductStoreDetails
                    {
                        StoreLocationId = location.StoreLocationId,
                        Stock = location.Stock,
                        DisplayName = location.DisplayName,
                        Address = location.Address,
                    };
                });
    }

    private sealed class StoreServiceProductStoreLocationDto
    {
        public int ProductId { get; set; }

        public int StoreLocationId { get; set; }

        public int Stock { get; set; }

        public string DisplayName { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;
    }
}
