using System.Net.Http.Headers;
using ECommerceBackend.Utils.Microservices;
using Microsoft.Extensions.Options;

namespace BFF.OrderFlow;

public class OrderFlowService(
    HttpClient httpClient,
    IOptions<MicroserviceHosts> hosts,
    ILogger<OrderFlowService> logger) : IOrderFlowService
{
    public async Task<HttpResponseMessage> CreateOrderPaymentSessionAsync(
        string? authorizationHeader,
        CancellationToken cancellationToken = default)
    {
        string url = $"{hosts.Value.OrderServiceUrl}/orderflow/session";
        using HttpRequestMessage request = CreateAuthorizedRequest(HttpMethod.Post, url, authorizationHeader);

        logger.LogDebug("Creating order payment session at {Url}", url);

        return await httpClient.SendAsync(request, cancellationToken);
    }

    private static HttpRequestMessage CreateAuthorizedRequest(
        HttpMethod method,
        string url,
        string? authorizationHeader)
    {
        var request = new HttpRequestMessage(method, url);

        if (!string.IsNullOrWhiteSpace(authorizationHeader))
        {
            request.Headers.Authorization = AuthenticationHeaderValue.Parse(authorizationHeader);
        }

        return request;
    }
}
