using MediatR;
using Microsoft.Extensions.Options;
using OrderService.Presentation.ServiceUseCases.Utils;

namespace OrderService.Presentation.ServiceUseCases.ProductDescription;

public class GetProductDescriptionHandler : IRequestHandler<GetProductDescriptionQuery, Result<List<ProductPriceModel>>>
{
    private readonly ILogger<GetProductDescriptionHandler> _logger;
    private readonly HttpClient _httpClient;
    private readonly MicroserviceNetworkConfig _microserviceNetworkConfig;

    public GetProductDescriptionHandler(ILogger<GetProductDescriptionHandler> logger,
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

        var query = new QueryString();
        foreach (int productId in request.ProductIds)
        {
            query = query.Add("ids", productId.ToString());
        }

        string productUrl = $"{_microserviceNetworkConfig.ProductServiceUrl}/product/by-ids{query}";
        HttpResponseMessage res = await _httpClient.GetAsync(productUrl, cancellationToken);
        if (!res.IsSuccessStatusCode)
        {
            _logger.LogError(
                "Failed to get product description. Url: {Url} Response: {@Response} ProductIds: {@ProductIds}",
                productUrl,
                res,
                request.ProductIds
            );
            return new Result<List<ProductPriceModel>>([
                new ResultError(
                    ResultErrorType.UNKNOWN_ERROR,
                    "Unknown error."
                )
            ]);
        }

        string sRes = await res.Content.ReadAsStringAsync(cancellationToken);
        _logger.LogDebug("Response: {}", sRes);
        List<ProductPriceModel>? productModels = JsonUtils.Deserialize<List<ProductPriceModel>>(sRes);
        return new Result<List<ProductPriceModel>>(productModels ?? new List<ProductPriceModel>());
    }
}