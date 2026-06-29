using System.Net;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OrderService.Application.UseCases.UserCart;
using OrderService.Application.Utils;

namespace OrderService.Application.UseCases.ProductReservation;

public class ProductReservationService : IProductReservationService
{
    private readonly HttpClient _httpClient;
    private readonly MicroserviceNetworkConfig _networkConfig;
    private readonly ILogger<ProductReservationService> _logger;

    public ProductReservationService(
        HttpClient httpClient,
        IOptions<MicroserviceNetworkConfig> networkConfig,
        ILogger<ProductReservationService> logger)
    {
        _httpClient = httpClient;
        _networkConfig = networkConfig.Value;
        _logger = logger;
    }

    public async Task<ResultError?> ReserveProductsAsync(
        Guid orderId,
        IEnumerable<StoreProductModel> cartProducts,
        CancellationToken cancellationToken = default)
    {
        _logger.LogTrace("Entered {Methodname}", nameof(ReserveProductsAsync));
        _logger.LogDebug("Reserving products for order {OrderId}", orderId);

        var requestBody = new
        {
            orderId,
            products = cartProducts.Select(cp => new
            {
                storeLocationId = cp.StoreLocation.StoreLocationId,
                productId = cp.Product.ProductId,
                stock = cp.Quantity
            })
        };

        string requestString = JsonUtils.Serialize(requestBody);
        using StringContent httpContent = new(requestString, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await _httpClient.PostAsync(
            $"{_networkConfig.StoreServiceUrl}/availableproducts/reserve",
            httpContent,
            cancellationToken);

        if (response.StatusCode == HttpStatusCode.Created)
        {
            _logger.LogDebug("Successfully reserved products for order {OrderId}", orderId);
            return null;
        }

        string body = await response.Content.ReadAsStringAsync(cancellationToken);
        _logger.LogError(
            "Failed to reserve products for order {OrderId}. Status: {StatusCode}. Body: {Body}",
            orderId,
            response.StatusCode,
            body);

        if (response.StatusCode == HttpStatusCode.Forbidden)
        {
            return new ResultError(
                ResultErrorType.INVALID_OPERATION_ERROR,
                "Failed to reserve products. The item might not be available anymore."
            );
        }

        return new ResultError(ResultErrorType.UNKNOWN_ERROR, "Failed to reserve products.");
    }
}
