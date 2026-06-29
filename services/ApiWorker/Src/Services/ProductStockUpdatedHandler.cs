using ApiWorker.Persistence;
using ApiWorker.Persistence.Entities;
using ECommerceBackend.EventTypes;
using Microsoft.EntityFrameworkCore;

namespace ApiWorker.Services;

public sealed class ProductStockUpdatedHandler(
    ReadDbContext readDbContext,
    ILogger<ProductStockUpdatedHandler> logger) : IEventHandler<ProductStockUpdatedEvent>
{
    public async Task HandleAsync(ProductStockUpdatedEvent ev, CancellationToken cancellationToken)
    {
        StoreProductReadEntity? entity = await readDbContext.StoreProducts
            .FindAsync([ev.StoreLocationId, ev.ProductId], cancellationToken);

        if (entity is null)
        {
            logger.LogWarning(
                "Store product not found for stock update: product {ProductId} at store location {StoreLocationId}",
                ev.ProductId,
                ev.StoreLocationId);
            return;
        }

        entity.Stock = ev.Stock;
        await readDbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Updated stock for product {ProductId} at store location {StoreLocationId} to {Stock}",
            ev.ProductId,
            ev.StoreLocationId,
            ev.Stock);
    }
}
