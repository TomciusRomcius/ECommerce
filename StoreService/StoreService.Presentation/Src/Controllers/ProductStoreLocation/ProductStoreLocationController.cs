using ECommerceBackend.Utils.Jwt;
using ECommerceBackend.Utils.Pagination;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreService.Application.UseCases.AvailableProducts.Queries;
using StoreService.Application.UseCases.ProductStoreLocations.Commands;
using GetProductsFromStoreQuery = StoreService.Application.UseCases.ProductStoreLocations.Queries.GetProductsFromStoreQuery;
using StoreService.Domain.Entities;
using StoreService.Presentation.Controllers.ProductStoreLocation.dtos;
using StoreService.Presentation.Utils;

namespace StoreService.Presentation.Controllers.ProductStoreLocation;

[ApiController]
[Route("[controller]")]
public class ProductStoreLocationController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductStoreLocationController(IMediator mediator)
    {
        _mediator = mediator;
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

        Page<ProductStoreLocationItemDto> response = new()
        {
            Data = page.Data.Select(ToDto).ToList(),
            TotalCount = page.TotalCount,
            HasNextPage = page.HasNextPage,
            HasPrevPage = page.HasPrevPage,
        };

        return Ok(response);
    }

    [HttpGet("by-product-ids")]
    public async Task<IActionResult> GetByProductIds([FromQuery] List<int> ids)
    {
        List<ProductStoreLocationDetails> result =
            await _mediator.Send(new GetProductsFromStoreWithIdsQuery(ids));
        return Ok(result);
    }

    private static ProductStoreLocationItemDto ToDto(ProductStoreLocationEntity entity) =>
        new()
        {
            StoreLocationId = entity.StoreLocationId,
            ProductId = entity.ProductId,
            Stock = entity.Stock,
        };

    [Authorize(Roles = RoleTypes.Admin)]
    [HttpPost]
    public async Task<IActionResult> AddProductToStore([FromBody] AddProductToStoreDto addProductToStoreDto)
    {
        var model = new ProductStoreLocationEntity(
            addProductToStoreDto.StoreLocationId,
            addProductToStoreDto.ProductId,
            addProductToStoreDto.Stock
        );

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
        var model = new ProductStoreLocationEntity(
            addProductToStoreDto.StoreLocationId,
            addProductToStoreDto.ProductId,
            addProductToStoreDto.Stock
        );

        await _mediator.Send(new UpdateProductStockCommand(model));
        return Ok();
    }
}