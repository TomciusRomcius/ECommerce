using System.Net.Http.Headers;
using System.Net.Http.Json;
using AutoMapper;
using BFF.ReadDb;
using BFF.StoreProducts;
using ECommerceBackend.Utils.Microservices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BFF.Cart;

public class CartService(
    IHttpClientFactory httpClientFactory,
    ReadDbContext readDbContext,
    IOptions<MicroserviceHosts> hosts,
    IMapper mapper) : ICartService
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient();

    public async Task<IReadOnlyList<CartItemWithProductDto>> GetItemsWithProductsAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        var query = readDbContext.CartProducts
            .AsNoTracking()
            .Where(cartProduct => cartProduct.UserId == userId)
            .Join(readDbContext.StoreProducts.AsNoTracking(),
                cartProduct => new { cartProduct.StoreLocationId, cartProduct.ProductId },
                storeProduct => new { storeProduct.StoreLocationId, storeProduct.ProductId },
                (cartProduct, storeProduct) => new { cartProduct, storeProduct })
            .Join(readDbContext.Products.AsNoTracking(),
                sp => sp.storeProduct.ProductId,
                product => product.ProductId,
                (sp, product) => new { sp.cartProduct, sp.storeProduct, product })
            .Join(readDbContext.Categories.AsNoTracking(),
                sp => sp.product.CategoryId,
                category => category.CategoryId,
                (sp, category) => new { sp.cartProduct, sp.storeProduct, sp.product, category })
            .Join(readDbContext.Manufacturers.AsNoTracking(),
                sp => sp.product.ManufacturerId,
                manufacturer => manufacturer.ManufacturerId,
                (sp, manufacturer) => new { sp.cartProduct, sp.storeProduct, sp.product, sp.category, manufacturer })
            .Join(readDbContext.StoreLocations.AsNoTracking(),
                sp => sp.storeProduct.StoreLocationId,
                storeLocation => storeLocation.StoreLocationId,
                (sp, storeLocation) => new
                {
                    sp.cartProduct,
                    sp.storeProduct,
                    sp.product,
                    sp.category,
                    sp.manufacturer,
                    storeLocation,
                })
            .Select(g => new
            {
                CartProduct = g.cartProduct,
                StoreProductRow = new StoreProductReadRow
                {
                    StoreProduct = g.storeProduct,
                    Product = g.product,
                    StoreLocation = g.storeLocation,
                    Manufacturer = g.manufacturer,
                    Category = g.category,
                    Images = readDbContext.ProductImages
                        .Where(image => image.ProductId == g.product.ProductId)
                        .ToList(),
                },
            });

        var rows = await query.ToListAsync(cancellationToken);

        return rows
            .Select(row => new CartItemWithProductDto
            {
                UserId = row.CartProduct.UserId,
                ProductId = row.CartProduct.ProductId,
                StoreLocationId = row.CartProduct.StoreLocationId,
                Quantity = row.CartProduct.Quantity,
                Product = mapper.Map<StoreProductDto>(row.StoreProductRow),
            })
            .ToList();
    }

    public async Task<HttpResponseMessage> AddItemAsync(
        AddCartItemRequest request,
        string? authorizationHeader,
        CancellationToken cancellationToken = default)
    {
        string cartUrl = $"{hosts.Value.UserServiceUrl}/cart";
        using HttpRequestMessage cartRequest = CreateAuthorizedRequest(HttpMethod.Post, cartUrl, authorizationHeader);
        cartRequest.Content = JsonContent.Create(new
        {
            productId = request.ProductId,
            storeLocationId = request.StoreLocationId,
            quantity = request.Quantity,
        });

        return await _httpClient.SendAsync(cartRequest, cancellationToken);
    }

    public async Task<HttpResponseMessage> RemoveItemAsync(
        int productId,
        int storeLocationId,
        string? authorizationHeader,
        CancellationToken cancellationToken = default)
    {
        var query = new QueryString()
            .Add("productId", productId.ToString())
            .Add("storeLocationId", storeLocationId.ToString());

        string cartUrl = $"{hosts.Value.UserServiceUrl}/cart{query}";
        using HttpRequestMessage cartRequest = CreateAuthorizedRequest(HttpMethod.Delete, cartUrl, authorizationHeader);
        return await _httpClient.SendAsync(cartRequest, cancellationToken);
    }

    private static HttpRequestMessage CreateAuthorizedRequest(
        HttpMethod method,
        string url,
        string? authorizationHeader)
    {
        var request = new HttpRequestMessage(method, url);

        if (!string.IsNullOrWhiteSpace(authorizationHeader))
        {
            request.Headers.Authorization = AuthenticationHeaderValue.Parse(authorizationHeader);
        }

        return request;
    }
}
