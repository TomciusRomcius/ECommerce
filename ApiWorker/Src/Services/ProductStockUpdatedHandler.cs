using ApiWorker.Persistence;
using ApiWorker.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApiWorker.Services;

public interface IProductStockUpdatedHandler
{
    Task HandleAsync(int storeLocationId, int productId, int stock, CancellationToken cancellationToken);
}

public sealed class ProductStockUpdatedHandler(
    ReadDbContext readDbContext,
    ILogger<ProductStockUpdatedHandler> logger) : IProductStockUpdatedHandler
{
    public async Task HandleAsync(
        int storeLocationId,
        int productId,
        int stock,
        CancellationToken cancellationToken)
    {
        StoreProductReadEntity? entity = await readDbContext.StoreProducts
            .FindAsync([storeLocationId, productId], cancellationToken);

        if (entity is null)
        {
            logger.LogWarning(
                "Store product not found for stock update: product {ProductId} at store location {StoreLocationId}",
                productId,
                storeLocationId);
            return;
        }

        entity.Stock = stock;
        await readDbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Updated stock for product {ProductId} at store location {StoreLocationId} to {Stock}",
            productId,
            storeLocationId,
            stock);
    }
}
