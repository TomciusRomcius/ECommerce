using System.Net.Http.Headers;
using ECommerceBackend.Utils.Jwt;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrderService.Application.Utils;

namespace OrderService.Application.UseCases.UserCart;

public class GetProductsFromUserCartHandler
    : IRequestHandler<GetProductsFromUserCartQuery, Result<List<CartProductMinimalModel>>>
{
    private readonly ILogger<GetProductsFromUserCartHandler> _logger;
    private readonly HttpClient _httpClient;
    private readonly MicroserviceNetworkConfig _microserviceNetworkConfig;
    private readonly JwtTokenReader _jwtTokenReader;

    public GetProductsFromUserCartHandler(
        ILogger<GetProductsFromUserCartHandler> logger,
        HttpClient httpClient,
        IOptions<MicroserviceNetworkConfig> microserviceNetworkConfig,
        JwtTokenReader jwtTokenReader)
    {
        _logger = logger;
        _httpClient = httpClient;
        _microserviceNetworkConfig = microserviceNetworkConfig.Value;
        _jwtTokenReader = jwtTokenReader;
    }

    public async Task<Result<List<CartProductMinimalModel>>> Handle(
        GetProductsFromUserCartQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogTrace("Entered Handle");
        _logger.LogDebug("Getting cart items from user: {UserId}", request.UserId);
        string cartUrl = $"{_microserviceNetworkConfig.UserServiceUrl}/cart/admin?userId={request.UserId}";
        var message = new HttpRequestMessage(HttpMethod.Get, cartUrl);
        message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _jwtTokenReader.AccessToken);
        HttpResponseMessage response = await _httpClient.SendAsync(message, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to get user cart items.");
            _logger.LogDebug("Failed to get user cart items. Response: {@Response}", response);
            return new Result<List<CartProductMinimalModel>>([
                new ResultError(ResultErrorType.UNKNOWN_ERROR, "Failed to get user cart items")
            ]);
        }

        string sResponse = await response.Content.ReadAsStringAsync(cancellationToken);
        List<CartProductMinimalModel>? cartProducts = JsonUtils.Deserialize<List<CartProductMinimalModel>>(sResponse);
        return new Result<List<CartProductMinimalModel>>(cartProducts ?? []);
    }
}
