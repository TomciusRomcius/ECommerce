using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.UseCases.ShippingAddress.Commands;
using UserService.Application.UseCases.ShippingAddress.Queries;
using UserService.Domain.Entities;
using UserService.Domain.Models;
using UserService.Presentation.Controllers.Address.dtos;

namespace UserService.Presentation.Controllers.Address;

[ApiController]
[Route("[controller]")]
public class AddressController : ControllerBase
{
    private readonly IMediator _mediator;

    public AddressController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAddresses()
    {
        string? userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId is null) return new UnauthorizedObjectResult("You must be logged in to get items!");

        List<ShippingAddressEntity> result = await _mediator.Send(new GetShippingAddressesQuery(new Guid(userId)));
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> AddAddress([FromBody] SetAddressDto setAddressDto)
    {
        string? userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId is null) return new UnauthorizedObjectResult("You must be logged in to get items!");

        var address = new ShippingAddressEntity
        {
            UserId = userId,
            RecipientName = setAddressDto.RecipientName,
            StreetAddress = setAddressDto.StreetAddress,
            ApartmentUnit = setAddressDto.ApartmentUnit,
            City = setAddressDto.City,
            State = setAddressDto.State,
            PostalCode = setAddressDto.PostalCode,
            Country = setAddressDto.Country,
            MobileNumber = setAddressDto.MobileNumber
        };

        await _mediator.Send(new AddShippingAddressCommand(address));
        return Created(nameof(AddAddress), null);
    }

    [HttpPatch]
    public async Task<IActionResult> UpdateAddress([FromBody] UpdateAddressDto updateAddressDto)
    {
        string? userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId is null) return new UnauthorizedObjectResult("You must be logged in to get items!");

        var updateModel = new UpdateShippingAddressModel
        {
            ShippingAddressId = updateAddressDto.ShippingAddressId,
            UserId = userId,
            RecipientName = updateAddressDto.RecipientName,
            StreetAddress = updateAddressDto.StreetAddress,
            ApartmentUnit = updateAddressDto.ApartmentUnit,
            City = updateAddressDto.City,
            State = updateAddressDto.State,
            PostalCode = updateAddressDto.PostalCode,
            Country = updateAddressDto.Country,
            MobileNumber = updateAddressDto.MobileNumber
        };

        await _mediator.Send(new UpdateShippingAddressCommand(updateModel));

        return Ok();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteAddress([FromBody] DeleteAddressDto deleteAddressDto)
    {
        string? userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId is null) return new UnauthorizedObjectResult("You must be logged in to get items!");

        await _mediator.Send(new RemoveShippingAddressCommand(new Guid(userId)));
        return Ok();
    }
}