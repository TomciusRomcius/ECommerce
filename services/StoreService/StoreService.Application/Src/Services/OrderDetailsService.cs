using System.Net.Http.Headers;
using ECommerceBackend.Utils.Database;
using ECommerceBackend.Utils.Jwt;
using ECommerceBackend.Utils.Microservices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace StoreService.Application.Services;

public class OrderDetailsService : IOrderDetailsService
{
    private readonly ILogger<OrderDetailsService> _logger;
    private readonly HttpClient _httpClient;
    private readonly MicroserviceHosts _serviceHosts;
    private readonly JwtTokenReader _jwtTokenReader;

    public OrderDetailsService(ILogger<OrderDetailsService> logger,
        HttpClient httpClient,
        IOptions<MicroserviceHosts> serviceHosts,
        JwtTokenReader jwtTokenReader)
    {
        _logger = logger;
        _httpClient = httpClient;
        _serviceHosts = serviceHosts.Value;
        _jwtTokenReader = jwtTokenReader;
    }

    public async Task<GetOrdersResponseType> FetchAsync(string userId, string orderId, CancellationToken cancellationToken)
    {
        _logger.LogTrace("Entered {FunctionName}", nameof(FetchAsync));
        HttpRequestMessage request = new(HttpMethod.Get, $"{_serviceHosts.OrderServiceUrl}/order/{orderId}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _jwtTokenReader.AccessToken);

        HttpResponseMessage response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
        string reqString = await response.Content.ReadAsStringAsync(cancellationToken);
        GetOrdersResponseType? responseDeserialized = JsonConvert.DeserializeObject<GetOrdersResponseType>(reqString);
        return responseDeserialized ?? throw new InvalidDataException($"Failed to deserialize request: {reqString}");
    }
}
