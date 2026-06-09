using System.Security.Claims;
using ECommerceBackend.Utils.Jwt;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderService.Application.Services;
using OrderService.Domain.Entities;
using OrderService.Presentation.ServiceUseCases.Payment;
using OrderService.Presentation.ServiceUseCases.Utils;

namespace OrderService.Presentation.UseCases.OrderFlow;

[ApiController]
[Route("[controller]")]
public class OrderFlowController : ControllerBase
{
    private readonly IOrderFlowService _orderFlowService;
    private readonly IOrderService _orderService;

    public OrderFlowController(IOrderFlowService orderFlowService, IOrderService orderService)
    {
        _orderFlowService = orderFlowService;
        _orderService = orderService;
    }

    [HttpGet("session")]
    public async Task<IActionResult> GetOrderPaymentSession()
    {
        // Fetch session id from db
        return Ok();
    }

    [HttpPost("session")]
    public async Task<IActionResult> CreateOrderPaymentSession()
    {
        string? userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        string? userJwt = await HttpContext.GetTokenAsync("access_token");
        if (String.IsNullOrWhiteSpace(userJwt))
            return Forbid();
        if (userId is null) return new UnauthorizedObjectResult("You must be logged in to add items to cart!");

        Result<PaymentSessionModel> result = await _orderFlowService.CreateOrderPaymentSession(new Guid(userId), PaymentProvider.STRIPE);
        if (result.Errors.Any())
        {
            return ControllerUtils.ResultErrorsToResponse(result.Errors);
        }
        return Ok(result.GetValue());
    }

    [HttpGet]
    [Authorize(Roles = RoleTypes.Admin)]
    public async Task<ActionResult<OrderEntity?>> GetOrder([FromQuery] Guid userId, [FromQuery] Guid orderId)
    {
        OrderEntity? order = await _orderService.GetOrderAsync(userId, orderId);
        return Ok(order);
    }
}
