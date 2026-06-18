using ApiWorker.Persistence;
using ApiWorker.Persistence.Entities;
using ECommerceBackend.EventTypes;
using Microsoft.EntityFrameworkCore;

namespace ApiWorker.Services;

public sealed class ProductAddedToStoreHandler(
    ReadDbContext readDbContext,
    ILogger<ProductAddedToStoreHandler> logger) : IEventHandler<ProductAddedToStoreEvent>
{
    public async Task HandleAsync(ProductAddedToStoreEvent ev, CancellationToken cancellationToken)
    {
        StoreProductReadEntity? existingAtStore = await readDbContext.StoreProducts
            .AsNoTracking()
            .FirstOrDefaultAsync(
                storeProduct => storeProduct.StoreLocationId == ev.StoreLocationId,
                cancellationToken);

        StoreProductReadEntity entity = new()
        {
            StoreLocationId = ev.StoreLocationId,
            ProductId = ev.ProductId,
            Stock = ev.Stock,
            StoreDisplayName = existingAtStore?.StoreDisplayName ?? string.Empty,
            StoreAddress = existingAtStore?.StoreAddress ?? string.Empty,
        };

        StoreProductReadEntity? existing = await readDbContext.StoreProducts
            .FindAsync([ev.StoreLocationId, ev.ProductId], cancellationToken);

        if (existing is null)
        {
            readDbContext.StoreProducts.Add(entity);
        }
        else
        {
            existing.Stock = entity.Stock;
            existing.StoreDisplayName = entity.StoreDisplayName;
            existing.StoreAddress = entity.StoreAddress;
        }

        await readDbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Persisted product {ProductId} at store location {StoreLocationId} with stock {Stock}",
            ev.ProductId,
            ev.StoreLocationId,
            ev.Stock);
    }
}
