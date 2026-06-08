using AutoMapper;
using ECommerceBackend.Utils.Jwt;
using ECommerceBackend.Utils.Pagination;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductService.Application.UseCases.Product.Commands;
using ProductService.Application.UseCases.Product.Queries;
using ProductService.Domain.Entities;
using ProductService.Domain.Utils;
using ProductService.Presentation.Controllers.Product.Dtos;
using ProductService.Presentation.Mapping;
using ProductService.Presentation.Utils;

namespace ProductService.Presentation.Controllers.Product;

[ApiController]
[Route("[controller]")]
public class ProductController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public ProductController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    /// <param name="pageNumber">Page number for pagination</param>
    /// <param name="pageSize">Number of products per page</param>
    [HttpGet]
    public async Task<IActionResult> GetProducts(string searchText = "", int pageNumber = 0, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        Page<ProductEntity> page = await _mediator.Send(new GetProductsQuery(searchText, pageNumber, pageSize), cancellationToken);

        Page<ProductDto> response = _mapper.MapPage<ProductEntity, ProductDto>(page);

        return Ok(response);
    }

    /// <param name="ids">Product ids</param>
    [HttpGet("by-ids")]
    public async Task<IActionResult> GetProductsByIds(
        [FromQuery] List<int> ids,
        CancellationToken cancellationToken = default)
    {
        Result<List<ProductEntity>> result = await _mediator.Send(new GetProductsByIdQuery(ids), cancellationToken);

        if (result.Errors.Any())
        {
            return ControllerUtils.ResultErrorToResponse(result.Errors.First());
        }

        List<ProductEntity> products = result.GetValue();
        return Ok(_mapper.Map<List<ProductDto>>(products));
    }

    [Authorize(Roles = RoleTypes.Admin)]
    [HttpPost]
    public async Task<IActionResult> CreateProducts([FromBody] RequestCreateProductDto createProductDto)
    {
        CreateProductCommand command = _mapper.Map<CreateProductCommand>(createProductDto);
        var result = await _mediator.Send(command);

        if (result.Errors.Any())
        {
            return ControllerUtils.ResultErrorToResponse(result.Errors.First());
        }

        ProductEntity product = result.GetValue();
        return Created(nameof(CreateProducts), new CreateProductResponseDto { ProductId = product.ProductId });
    }
}
