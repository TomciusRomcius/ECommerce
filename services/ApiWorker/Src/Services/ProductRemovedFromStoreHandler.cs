using ApiWorker.Persistence;
using ApiWorker.Persistence.Entities;
using ECommerceBackend.EventTypes;
using Microsoft.EntityFrameworkCore;

namespace ApiWorker.Services;

public sealed class ProductRemovedFromStoreHandler(
    ReadDbContext readDbContext,
    ILogger<ProductRemovedFromStoreHandler> logger) : IEventHandler<ProductRemovedFromStoreEvent>
{
    public async Task HandleAsync(ProductRemovedFromStoreEvent ev, CancellationToken cancellationToken)
    {
        StoreProductReadEntity? entity = await readDbContext.StoreProducts
            .FindAsync([ev.StoreLocationId, ev.ProductId], cancellationToken);

        if (entity is null)
        {
            logger.LogWarning(
                "Store product not found for removal: product {ProductId} at store location {StoreLocationId}",
                ev.ProductId,
                ev.StoreLocationId);
            return;
        }

        readDbContext.StoreProducts.Remove(entity);
        await readDbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Removed product {ProductId} from store location {StoreLocationId} in read model",
            ev.ProductId,
            ev.StoreLocationId);
    }
}
