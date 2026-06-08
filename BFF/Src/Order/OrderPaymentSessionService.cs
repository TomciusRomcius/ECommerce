using System.Net.Http.Headers;
using ECommerceBackend.Utils.Microservices;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace BFF.Order;

public class OrderPaymentSessionService(HttpClient httpClient, IOptions<MicroserviceHosts> hosts)
    : IOrderPaymentSessionService
{
    public async Task<HttpResponseMessage> CreateOrderPaymentSessionAsync(
        bool testCharge,
        string? authorizationHeader,
        CancellationToken cancellationToken = default)
    {
        string url = $"{hosts.Value.OrderServiceUrl}/orderflow/session";

        using var request = new HttpRequestMessage(HttpMethod.Post, url);

        if (!string.IsNullOrWhiteSpace(authorizationHeader))
        {
            request.Headers.Authorization = AuthenticationHeaderValue.Parse(authorizationHeader);
        }

        return await httpClient.SendAsync(request, cancellationToken);
    }
}
