using BFF.Utils;
using ECommerceBackend.Utils.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BFF.StoreProducts;

[ApiController]
[Route("[controller]")]
public class StoreProductsController(
    IStoreProductsService storeProductsService,
    ILogger<StoreProductsController> logger) : ControllerBase
{
    /// <summary>
    /// Returns store products from the ReadDB.
    /// When storeLocationId is set, only products stocked at that store are included.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetProducts(
        [FromQuery] int? storeLocationId,
        [FromQuery] int pageNumber = 0,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        try
        {
            Page<StoreProductDto> page = await storeProductsService.GetProductsAsync(
                storeLocationId,
                pageNumber,
                pageSize,
                cancellationToken);

            return Ok(new { data = page });
        }
        catch (HttpRequestException ex)
        {
            logger.LogWarning(ex, "Failed to fetch products (storeLocationId={StoreLocationId}).", storeLocationId);
            return StatusCode(StatusCodes.Status502BadGateway, new { error = "Failed to fetch products." });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to read products from ReadDB (storeLocationId={StoreLocationId}).", storeLocationId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = "Failed to fetch products." });
        }
    }

    [HttpGet("{productId:int}")]
    public async Task<IActionResult> GetProduct(int productId, CancellationToken cancellationToken = default)
    {
        try
        {
            StoreProductDto? product = await storeProductsService.GetProductByIdAsync(productId, cancellationToken);

            if (product is null)
            {
                return NotFound();
            }

            return Ok(new { data = product });
        }
        catch (HttpRequestException ex)
        {
            logger.LogWarning(ex, "Failed to fetch product {ProductId}.", productId);
            return StatusCode(StatusCodes.Status502BadGateway, new { error = "Failed to fetch product." });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to read product {ProductId} from ReadDB.", productId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = "Failed to fetch product." });
        }
    }

    [HttpPut]
    [Authorize]
    public async Task<IActionResult> UpdateProductStock(
        [FromBody] UpdateProductStockRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using HttpResponseMessage response = await storeProductsService.UpdateProductStockAsync(
                request.StoreLocationId,
                request.ProductId,
                request.Stock,
                Request.Headers.Authorization.ToString(),
                cancellationToken);

            string body = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning(
                    "Update product stock failed with status {StatusCode}: {Body}",
                    response.StatusCode,
                    body);
            }

            return HttpResponseUtils.FromStringBody((int)response.StatusCode, body);
        }
        catch (HttpRequestException ex)
        {
            logger.LogWarning(
                ex,
                "Failed to update stock for product {ProductId} at store {StoreLocationId}.",
                request.ProductId,
                request.StoreLocationId);
            return StatusCode(StatusCodes.Status502BadGateway, new { error = "Failed to update product stock." });
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddProductToStore(
        [FromBody] UpdateProductStockRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using HttpResponseMessage response = await storeProductsService.AddProductToStoreAsync(
                request.StoreLocationId,
                request.ProductId,
                request.Stock,
                Request.Headers.Authorization.ToString(),
                cancellationToken);

            string body = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning(
                    "Add product to store failed with status {StatusCode}: {Body}",
                    response.StatusCode,
                    body);
            }

            return HttpResponseUtils.FromStringBody((int)response.StatusCode, body);
        }
        catch (HttpRequestException ex)
        {
            logger.LogWarning(
                ex,
                "Failed to add product {ProductId} to store {StoreLocationId}.",
                request.ProductId,
                request.StoreLocationId);
            return StatusCode(StatusCodes.Status502BadGateway, new { error = "Failed to add product to store." });
        }
    }
}
