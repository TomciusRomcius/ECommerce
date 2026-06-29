using System.Security.Claims;
using ECommerceBackend.Utils.Jwt;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderService.Application.Services;
using OrderService.Application.UseCases.OrderFlow;
using OrderService.Application.UseCases.Payment;
using OrderService.Application.Utils;
using OrderService.Domain.Entities;
using OrderService.Presentation.Utils;

namespace OrderService.Presentation.Controllers.OrderFlow;

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
    public Task<IActionResult> GetOrderPaymentSession()
    {
        return Task.FromResult<IActionResult>(Ok());
    }

    [HttpPost("session")]
    public async Task<IActionResult> CreateOrderPaymentSession()
    {
        string? userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        string? userJwt = await HttpContext.GetTokenAsync("access_token");
        if (string.IsNullOrWhiteSpace(userJwt))
            return Forbid();
        if (userId is null)
            return new UnauthorizedObjectResult("You must be logged in to add items to cart!");

        Result<PaymentSessionModel> result =
            await _orderFlowService.CreateOrderPaymentSession(new Guid(userId), PaymentProvider.STRIPE);
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
