using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrderService.Application.Utils;

namespace OrderService.Application.UseCases.ProductDescription;

public class GetProductDescriptionHandler
    : IRequestHandler<GetProductDescriptionQuery, Result<List<ProductPriceModel>>>
{
    private readonly ILogger<GetProductDescriptionHandler> _logger;
    private readonly HttpClient _httpClient;
    private readonly MicroserviceNetworkConfig _microserviceNetworkConfig;

    public GetProductDescriptionHandler(
        ILogger<GetProductDescriptionHandler> logger,
        HttpClient httpClient,
        IOptions<MicroserviceNetworkConfig> microserviceNetworkConfig)
    {
        _logger = logger;
        _httpClient = httpClient;
        _microserviceNetworkConfig = microserviceNetworkConfig.Value;
    }

    public async Task<Result<List<ProductPriceModel>>> Handle(
        GetProductDescriptionQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogTrace("Entered Handle");

        if (request.ProductIds.Count == 0)
        {
            return new Result<List<ProductPriceModel>>(new List<ProductPriceModel>());
        }

        string idsQuery = string.Join("&", request.ProductIds.Select(id => $"ids={id}"));
        string productUrl = $"{_microserviceNetworkConfig.ProductServiceUrl}/product/by-ids?{idsQuery}";
        HttpResponseMessage res = await _httpClient.GetAsync(productUrl, cancellationToken);
        if (!res.IsSuccessStatusCode)
        {
            _logger.LogError(
                "Failed to get product description. Url: {Url} Response: {@Response} ProductIds: {@ProductIds}",
                productUrl,
                res,
                request.ProductIds);
            return new Result<List<ProductPriceModel>>([
                new ResultError(ResultErrorType.UNKNOWN_ERROR, "Unknown error.")
            ]);
        }

        string sRes = await res.Content.ReadAsStringAsync(cancellationToken);
        _logger.LogDebug("Response: {}", sRes);
        List<ProductPriceModel>? productModels = JsonUtils.Deserialize<List<ProductPriceModel>>(sRes);
        return new Result<List<ProductPriceModel>>(productModels ?? []);
    }
}
