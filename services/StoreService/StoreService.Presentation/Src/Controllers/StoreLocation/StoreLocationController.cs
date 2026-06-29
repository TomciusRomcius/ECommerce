using AutoMapper;
using ECommerceBackend.Utils.Jwt;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreService.Application.UseCases.StoreLocation.Commands;
using StoreService.Application.UseCases.StoreLocation.Queries;
using StoreService.Domain.Entities;
using StoreService.Domain.Models;
using StoreService.Presentation.Controllers.StoreLocation.dtos;

namespace StoreService.Presentation.Controllers.StoreLocation;

[ApiController]
[Route("[controller]")]
public class StoreLocation : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public StoreLocation(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetLocations([FromQuery] int pageNumber)
    {
        List<StoreLocationEntity> result = await _mediator.Send(new GetStoreLocationsQuery(pageNumber));
        return Ok(result);
    }

    [HttpGet("{storeLocationId:int}")]
    public async Task<IActionResult> GetLocation(int storeLocationId)
    {
        StoreLocationEntity? result = await _mediator.Send(new GetStoreLocationByIdQuery(storeLocationId));
        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [Authorize(Roles = RoleTypes.Admin)]
    [HttpPost]
    public async Task<IActionResult> CreateStoreLocation([FromBody] RequestCreateLocationDto createLocationDto)
    {
        var result = await _mediator.Send(new CreateStoreLocationCommand(
            _mapper.Map<CreateStoreLocationModel>(createLocationDto)
        ));

        return Created(nameof(CreateStoreLocation), result);
    }

    [Authorize(Roles = RoleTypes.Admin)]
    [HttpPatch]
    public async Task<IActionResult> ModifyStoreLocation([FromBody] RequestModifyLocationDto modifyLocationDto)
    {
        await _mediator.Send(new UpdateStoreLocationCommand(
            _mapper.Map<UpdateStoreLocationModel>(modifyLocationDto)
        ));

        return Ok();
    }

    [Authorize(Roles = RoleTypes.Admin)]
    [HttpDelete]
    public async Task<IActionResult> RemoveLocation([FromBody] RequestRemoveLocationDto removeLocationDto)
    {
        await _mediator.Send(new RemoveStoreLocationCommand(removeLocationDto.StoreLocationId));
        return Ok();
    }
}
