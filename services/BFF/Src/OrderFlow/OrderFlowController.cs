using BFF.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BFF.OrderFlow;

[ApiController]
[Route("[controller]")]
public class OrderFlowController(IOrderFlowService orderFlowService, ILogger<OrderFlowController> logger)
    : ControllerBase
{
    [HttpPost("session")]
    [Authorize]
    public async Task<IActionResult> CreateOrderPaymentSession(CancellationToken cancellationToken)
    {
        string authorizationHeader = Request.Headers.Authorization.ToString();

        using HttpResponseMessage response =
            await orderFlowService.CreateOrderPaymentSessionAsync(authorizationHeader, cancellationToken);

        string body = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            logger.LogWarning(
                "Order flow session creation failed with status {StatusCode}: {Body}",
                response.StatusCode,
                body);
        }

        return HttpResponseUtils.FromStringBody((int)response.StatusCode, body);
    }
}
