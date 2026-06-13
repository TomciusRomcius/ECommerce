using ApiWorker.Persistence;
using ApiWorker.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApiWorker.Services;

public interface IProductRemovedFromStoreHandler
{
    Task HandleAsync(int storeLocationId, int productId, CancellationToken cancellationToken);
}

public sealed class ProductRemovedFromStoreHandler(
    ReadDbContext readDbContext,
    ILogger<ProductRemovedFromStoreHandler> logger) : IProductRemovedFromStoreHandler
{
    public async Task HandleAsync(
        int storeLocationId,
        int productId,
        CancellationToken cancellationToken)
    {
        StoreProductReadEntity? entity = await readDbContext.StoreProducts
            .FindAsync([storeLocationId, productId], cancellationToken);

        if (entity is null)
        {
            logger.LogWarning(
                "Store product not found for removal: product {ProductId} at store location {StoreLocationId}",
                productId,
                storeLocationId);
            return;
        }

        readDbContext.StoreProducts.Remove(entity);
        await readDbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Removed product {ProductId} from store location {StoreLocationId} in read model",
            productId,
            storeLocationId);
    }
}
