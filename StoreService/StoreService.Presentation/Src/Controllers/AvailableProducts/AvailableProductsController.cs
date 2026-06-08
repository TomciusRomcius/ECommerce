using AutoMapper;
using ECommerceBackend.Utils.Jwt;
using ECommerceBackend.Utils.Pagination;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreService.Application.UseCases.AvailableProducts.Queries;
using StoreService.Application.UseCases.ProductStoreLocations.Commands;
using StoreService.Domain.Entities;
using StoreService.Presentation.Controllers.AvailableProducts.dtos;
using StoreService.Presentation.Mapping;
using StoreService.Presentation.Utils;

namespace StoreService.Presentation.Controllers.AvailableProducts;

[ApiController]
[Route("[controller]")]
public class AvailableProductsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public AvailableProductsController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    /// <param name="query">Store location id and pagination parameters</param>
    [HttpGet]
    public async Task<IActionResult> GetProductsFromStore(
        [FromQuery] GetProductsFromStoreDto query,
        CancellationToken cancellationToken = default)
    {
        Page<ProductStoreLocationEntity> page = await _mediator.Send(
            new GetProductsFromStoreQuery(query.StoreLocationId, query.PageNumber, query.PageSize),
            cancellationToken);

        Page<ProductStoreLocationItemDto> response =
            _mapper.MapPage<ProductStoreLocationEntity, ProductStoreLocationItemDto>(page);

        return Ok(response);
    }

    [HttpGet("by-product-ids")]
    public async Task<IActionResult> GetByProductIds([FromQuery] List<int> ids)
    {
        List<ProductStoreLocationDetails> result =
            await _mediator.Send(new GetProductsFromStoreWithIdsQuery(ids));
        return Ok(result);
    }

    [Authorize(Roles = RoleTypes.Admin)]
    [HttpPost]
    public async Task<IActionResult> AddProductToStore([FromBody] AddProductToStoreDto addProductToStoreDto)
    {
        var model = _mapper.Map<ProductStoreLocationEntity>(addProductToStoreDto);

        var error = await _mediator.Send(new AddProductToStoreCommand(model));
        return error == null ? Ok() : ControllerUtils.ResultErrorToResponse(error);
    }

    [Authorize(Roles = RoleTypes.Admin)]
    [HttpDelete]
    public async Task<IActionResult> RemoveProductFromStore(
        [FromBody] RemoveProductFromStoreDto removeProductFromStoreDto)
    {
        await _mediator.Send(
            new RemoveProductFromStoreCommand(removeProductFromStoreDto.StoreLocationId,
                removeProductFromStoreDto.ProductId
            ));
        return Ok();
    }

    [Authorize(Roles = RoleTypes.Admin)]
    [HttpPut]
    public async Task<IActionResult> ModifyProductFromStore([FromBody] AddProductToStoreDto addProductToStoreDto)
    {
        ProductStoreLocationEntity model = _mapper.Map<ProductStoreLocationEntity>(addProductToStoreDto);

        await _mediator.Send(new UpdateProductStockCommand(model));
        return Ok();
    }
}
