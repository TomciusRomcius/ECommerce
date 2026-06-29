using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ECommerceBackend.Utils.Jwt;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.UseCases.Cart.Commands;
using UserService.Application.UseCases.Cart.Queries;
using UserService.Domain.Entities;
using UserService.Domain.Utils;
using UserService.Presentation.Controllers.Cart.dtos;
using UserService.Presentation.Utils;

namespace UserService.Presentation.Controllers.Cart;

[ApiController]
[Route("[controller]")]
public class CartController : ControllerBase
{
    private readonly ILogger<CartController> _logger;
    private readonly IMediator _mediator;

    public CartController(ILogger<CartController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetItems()
    {
        _logger.LogInformation("User: {@User}", HttpContext.User.Claims);
        string? userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId is null) return new UnauthorizedObjectResult("You must be logged in to get items!");

        Result<List<CartProductEntity>> result = await _mediator.Send(new GetUserCartItemsQuery(new Guid(userId)));

        if (result.Errors.Any())
        {
            return ControllerUtils.ResultErrorToResponse(result.Errors.First());
        }

        return Ok(result.GetValue());
    }

    [HttpGet("admin")]
    [Authorize(Roles = RoleTypes.Admin)]
    public async Task<IActionResult> GetItemsForUser([FromQuery] Guid userId)
    {
        Result<List<CartProductEntity>> result = await _mediator.Send(new GetUserCartItemsQuery(userId));

        if (result.Errors.Any())
        {
            return ControllerUtils.ResultErrorToResponse(result.Errors.First());
        }

        return Ok(result.GetValue());
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddItem([FromBody] RequestAddItemDto addItemDto)
    {
        string userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value;

        ResultError? error = await _mediator.Send(new AddItemToCartCommand(
            new CartProductEntity(userId, addItemDto.ProductId, addItemDto.StoreLocationId, addItemDto.Quantity)
        ));

        if (error != null)
        {
            return ControllerUtils.ResultErrorToResponse(error);
        }

        return Created(nameof(AddItem), null);
    }

    [HttpPatch]
    [Authorize]
    public async Task<IActionResult> UpdateItemQuantity([FromBody] RequestAddItemDto addItemDto)
    {
        string userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)!.Value;
        ResultError? error = await _mediator.Send(new UpdateCartItemQuantityCommand(
            new CartProductEntity(userId, addItemDto.ProductId, addItemDto.StoreLocationId, addItemDto.Quantity)
        ));

        if (error != null)
        {
            return ControllerUtils.ResultErrorToResponse(error);
        }

        return Ok();
    }

    [HttpDelete]
    [Authorize]
    public async Task<IActionResult> RemoveItem([FromQuery] int productId, [FromQuery] int storeLocationId)
    {
        string? userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId is null) return new UnauthorizedObjectResult("You must be logged in to remove items!");

        ResultError? error = await _mediator.Send(new RemoveItemFromCartCommand(userId, productId, storeLocationId));

        if (error != null)
        {
            return ControllerUtils.ResultErrorToResponse(error);
        }

        return Ok();
    }
}