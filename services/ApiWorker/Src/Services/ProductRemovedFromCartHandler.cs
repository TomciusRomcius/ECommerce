using ApiWorker.Persistence;
using ApiWorker.Persistence.Entities;
using ECommerceBackend.EventTypes;

namespace ApiWorker.Services;

public sealed class ProductRemovedFromCartHandler(
    ReadDbContext readDbContext,
    ILogger<ProductRemovedFromCartHandler> logger) : IEventHandler<ProductRemovedFromCartEvent>
{
    public async Task HandleAsync(ProductRemovedFromCartEvent ev, CancellationToken cancellationToken)
    {
        CartProductReadEntity? entity = await readDbContext.CartProducts
            .FindAsync([ev.UserId, ev.StoreLocationId, ev.ProductId], cancellationToken);

        if (entity is null)
        {
            logger.LogWarning(
                "Cart item not found for removal: user {UserId}, product {ProductId} at store {StoreLocationId}",
                ev.UserId,
                ev.ProductId,
                ev.StoreLocationId);
            return;
        }

        readDbContext.CartProducts.Remove(entity);
        await readDbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Removed cart item for user {UserId}: product {ProductId} at store {StoreLocationId}",
            ev.UserId,
            ev.ProductId,
            ev.StoreLocationId);
    }
}
