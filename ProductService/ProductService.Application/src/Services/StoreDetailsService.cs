using System.Net.Http.Json;
using AutoMapper;
using ECommerceBackend.Utils.Microservices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ProductService.Application.Services;

public class StoreDetailsService(
    HttpClient httpClient,
    IOptions<MicroserviceHosts> hosts,
    ILogger<StoreDetailsService> logger,
    IMapper mapper) : IStoreDetailsService
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
                group => mapper.Map<ProductStoreDetails>(group.First()));
    }
}
