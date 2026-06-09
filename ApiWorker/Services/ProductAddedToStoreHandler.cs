using ApiWorker.Persistence;
using ApiWorker.Persistence.Entities;
using ECommerceBackend.Utils.Microservices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json;

namespace ApiWorker.Services;

public interface IProductAddedToStoreHandler
{
    Task HandleAsync(int storeLocationId, int productId, int stock, CancellationToken cancellationToken);
}

public sealed class ProductAddedToStoreHandler(
    IHttpClientFactory httpClientFactory,
    IOptions<MicroserviceHosts> microserviceHosts,
    ReadDbContext readDbContext,
    ILogger<ProductAddedToStoreHandler> logger) : IProductAddedToStoreHandler
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient();

    public async Task HandleAsync(
        int storeLocationId,
        int productId,
        int stock,
        CancellationToken cancellationToken)
    {
        StoreLocationDto storeLocation = await FetchStoreLocationAsync(storeLocationId, cancellationToken);
        ProductDto product = await FetchProductAsync(productId, cancellationToken);

        StoreProductReadEntity entity = new()
        {
            StoreLocationId = storeLocationId,
            ProductId = productId,
            Stock = stock,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            ManufacturerId = product.ManufacturerId,
            CategoryId = product.CategoryId,
            ManufacturerName = product.Manufacturer?.Name ?? string.Empty,
            CategoryName = product.Category?.Name ?? string.Empty,
            StoreDisplayName = storeLocation.DisplayName,
            StoreAddress = storeLocation.Address,
        };

        StoreProductReadEntity? existing = await readDbContext.StoreProducts
            .FindAsync([storeLocationId, productId], cancellationToken);

        if (existing is null)
        {
            readDbContext.StoreProducts.Add(entity);
        }
        else
        {
            existing.Stock = entity.Stock;
            existing.Name = entity.Name;
            existing.Description = entity.Description;
            existing.Price = entity.Price;
            existing.ManufacturerId = entity.ManufacturerId;
            existing.CategoryId = entity.CategoryId;
            existing.ManufacturerName = entity.ManufacturerName;
            existing.CategoryName = entity.CategoryName;
            existing.StoreDisplayName = entity.StoreDisplayName;
            existing.StoreAddress = entity.StoreAddress;
        }

        await SyncProductImagesAsync(productId, product.ImageKeys, cancellationToken);
        await readDbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Persisted product {ProductId} at store location {StoreLocationId} with stock {Stock}",
            productId,
            storeLocationId,
            stock);
    }

    private async Task SyncProductImagesAsync(
        int productId,
        IReadOnlyList<string> imageKeys,
        CancellationToken cancellationToken)
    {
        List<ProductImageReadEntity> existingImages = await readDbContext.ProductImages
            .Where(image => image.ProductId == productId)
            .ToListAsync(cancellationToken);

        HashSet<string> incomingKeys = imageKeys
            .Where(key => !string.IsNullOrWhiteSpace(key))
            .ToHashSet(StringComparer.Ordinal);

        List<ProductImageReadEntity> imagesToRemove = existingImages
            .Where(image => !incomingKeys.Contains(image.S3Key))
            .ToList();

        if (imagesToRemove.Count > 0)
        {
            readDbContext.ProductImages.RemoveRange(imagesToRemove);
        }

        HashSet<string> existingKeys = existingImages
            .Select(image => image.S3Key)
            .ToHashSet(StringComparer.Ordinal);

        foreach (string key in incomingKeys.Where(key => !existingKeys.Contains(key)))
        {
            readDbContext.ProductImages.Add(new ProductImageReadEntity(productId, key));
        }
    }

    private async Task<StoreLocationDto> FetchStoreLocationAsync(
        int storeLocationId,
        CancellationToken cancellationToken)
    {
        string url = $"{microserviceHosts.Value.StoreServiceUrl.TrimEnd('/')}/StoreLocation/{storeLocationId}";
        logger.LogDebug("Fetching store location from {Url}", url);

        using HttpResponseMessage response = await _httpClient.GetAsync(url, cancellationToken);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            throw new InvalidOperationException($"Store location {storeLocationId} was not found.");
        }

        response.EnsureSuccessStatusCode();

        StoreLocationDto? storeLocation =
            await response.Content.ReadFromJsonAsync<StoreLocationDto>(JsonOptions, cancellationToken);

        if (storeLocation is null)
        {
            throw new InvalidOperationException($"Store location {storeLocationId} response was malformed.");
        }

        return storeLocation;
    }

    private async Task<ProductDto> FetchProductAsync(int productId, CancellationToken cancellationToken)
    {
        string url =
            $"{microserviceHosts.Value.ProductServiceUrl.TrimEnd('/')}/Product/by-ids?ids={productId}";
        logger.LogDebug("Fetching product from {Url}", url);

        using HttpResponseMessage response = await _httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();

        List<ProductDto>? products =
            await response.Content.ReadFromJsonAsync<List<ProductDto>>(JsonOptions, cancellationToken);

        ProductDto? product = products?.FirstOrDefault();
        if (product is null)
        {
            throw new InvalidOperationException($"Product {productId} was not found.");
        }

        return product;
    }
}
