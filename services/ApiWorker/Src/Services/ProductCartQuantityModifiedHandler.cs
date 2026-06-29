using ApiWorker.Persistence;
using ApiWorker.Persistence.Entities;
using ECommerceBackend.EventTypes;

namespace ApiWorker.Services;

public sealed class ProductCartQuantityModifiedHandler(
    ReadDbContext readDbContext,
    ILogger<ProductCartQuantityModifiedHandler> logger) : IEventHandler<ProductCartQuantityModifiedEvent>
{
    public async Task HandleAsync(ProductCartQuantityModifiedEvent ev, CancellationToken cancellationToken)
    {
        CartProductReadEntity? entity = await readDbContext.CartProducts
            .FindAsync([ev.UserId, ev.StoreLocationId, ev.ProductId], cancellationToken);

        if (entity is null)
        {
            logger.LogWarning(
                "Cart item not found for quantity update: user {UserId}, product {ProductId} at store {StoreLocationId}",
                ev.UserId,
                ev.ProductId,
                ev.StoreLocationId);
            return;
        }

        entity.Quantity = ev.Quantity;
        await readDbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Updated cart item quantity for user {UserId}: product {ProductId} at store {StoreLocationId} to {Quantity}",
            ev.UserId,
            ev.ProductId,
            ev.StoreLocationId,
            ev.Quantity);
    }
}
