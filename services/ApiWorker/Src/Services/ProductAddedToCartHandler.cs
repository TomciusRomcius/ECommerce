using ApiWorker.Persistence;
using ApiWorker.Persistence.Entities;
using ECommerceBackend.EventTypes;

namespace ApiWorker.Services;

public sealed class ProductAddedToCartHandler(
    ReadDbContext readDbContext,
    ILogger<ProductAddedToCartHandler> logger) : IEventHandler<ProductAddedToCartEvent>
{
    public async Task HandleAsync(ProductAddedToCartEvent ev, CancellationToken cancellationToken)
    {
        readDbContext.CartProducts.Add(new CartProductReadEntity
        {
            UserId = ev.UserId,
            StoreLocationId = ev.StoreLocationId,
            ProductId = ev.ProductId,
            Quantity = ev.Quantity,
        });

        await readDbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Persisted cart item for user {UserId}: product {ProductId} at store {StoreLocationId} with quantity {Quantity}",
            ev.UserId,
            ev.ProductId,
            ev.StoreLocationId,
            ev.Quantity);
    }
}
