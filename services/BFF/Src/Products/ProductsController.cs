using BFF.Utils;
using ECommerceBackend.Utils.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BFF.Products;

[ApiController]
[Route("[controller]")]
public class ProductsController(IProductService productService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetProducts(
        string searchText = "",
        int pageNumber = 0,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        Result<Page<ProductWithImageUrlsDto>> result =
            await productService.GetProductsAsync(searchText, pageNumber, pageSize, cancellationToken);

        if (result.Errors.Any())
        {
            return ControllerUtils.ResultErrorsToResponse(result.Errors);
        }

        return Ok(result.GetValue());
    }

    [HttpGet("{productId:int}")]
    public async Task<IActionResult> GetProduct(int productId, CancellationToken cancellationToken = default)
    {
        Result<ProductWithImageUrlsDto> result =
            await productService.GetProductAsync(productId, cancellationToken);

        if (result.Errors.Any())
        {
            return ControllerUtils.ResultErrorsToResponse(result.Errors);
        }

        return Ok(result.GetValue());
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateProduct(
        [FromForm] CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        string authorizationHeader = Request.Headers.Authorization.ToString();

        using HttpResponseMessage response =
            await productService.CreateProductAsync(request, authorizationHeader, cancellationToken);

        string body = await response.Content.ReadAsStringAsync(cancellationToken);
        return HttpResponseUtils.FromStringBody((int)response.StatusCode, body);
    }
}
