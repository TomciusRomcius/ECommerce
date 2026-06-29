using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrderService.Application.Utils;

namespace OrderService.Application.UseCases.StoreProducts;

public class GetStoreProductsHandler
    : IRequestHandler<GetStoreProductsQuery, Result<List<StoreProductLocationModel>>>
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<GetStoreProductsHandler> _logger;
    private readonly MicroserviceNetworkConfig _microserviceNetworkConfig;

    public GetStoreProductsHandler(
        ILogger<GetStoreProductsHandler> logger,
        HttpClient httpClient,
        IOptions<MicroserviceNetworkConfig> microserviceNetworkConfig)
    {
        _logger = logger;
        _httpClient = httpClient;
        _microserviceNetworkConfig = microserviceNetworkConfig.Value;
    }

    public async Task<Result<List<StoreProductLocationModel>>> Handle(
        GetStoreProductsQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogTrace("Entered Handle");

        if (request.ProductIds.Count == 0)
        {
            return new Result<List<StoreProductLocationModel>>(new List<StoreProductLocationModel>());
        }

        string idsQuery = string.Join("&", request.ProductIds.Select(id => $"ids={id}"));
        string storeProductsUrl =
            $"{_microserviceNetworkConfig.StoreServiceUrl}/availableproducts/by-product-ids?{idsQuery}";
        HttpResponseMessage response = await _httpClient.GetAsync(storeProductsUrl, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError(
                "Failed to get store products. Url: {Url} Response: {@Response} ProductIds: {@ProductIds}",
                storeProductsUrl,
                response,
                request.ProductIds);
            return new Result<List<StoreProductLocationModel>>([
                new ResultError(ResultErrorType.UNKNOWN_ERROR, "Unknown error.")
            ]);
        }

        string responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
        _logger.LogDebug("Response: {}", responseBody);
        List<StoreProductLocationModel>? storeProducts =
            JsonUtils.Deserialize<List<StoreProductLocationModel>>(responseBody);
        return new Result<List<StoreProductLocationModel>>(storeProducts ?? []);
    }
}
