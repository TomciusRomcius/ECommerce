using System.Net.Http.Headers;
using BFF.Utils;
using ECommerceBackend.Utils.Jwt;
using ECommerceBackend.Utils.Microservices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BFF.Checkout;

[ApiController]
[Route("[controller]")]
public class CheckoutController(IOptions<MicroserviceHosts> hosts, HttpClient httpClient, JwtTokenReader jwtTokenReader) : ControllerBase {
    [HttpPost("verify")]
    public async Task<IActionResult> VerifyPaymentAsync([FromBody] VerifyCheckoutRequest request, 
        CancellationToken ct = default)
    {
        string upstream = $"{hosts.Value.PaymentServiceUrl.TrimEnd('/')}/paymentsession/verify/stripe";

        using HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, upstream);
        message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtTokenReader.AccessToken);
        message.Content = JsonContent.Create(request);

        using HttpResponseMessage response = await httpClient.SendAsync(message, ct);
        return HttpResponseUtils.FromStringBody((int)response.StatusCode, await response.Content.ReadAsStringAsync(ct));
    }
}
