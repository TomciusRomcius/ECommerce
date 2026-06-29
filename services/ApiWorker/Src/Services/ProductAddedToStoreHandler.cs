using ApiWorker.Persistence;
using ApiWorker.Persistence.Entities;
using ECommerceBackend.EventTypes;

namespace ApiWorker.Services;

public sealed class ProductAddedToStoreHandler(
    ReadDbContext readDbContext,
    ILogger<ProductAddedToStoreHandler> logger) : IEventHandler<ProductAddedToStoreEvent>
{
    public async Task HandleAsync(ProductAddedToStoreEvent ev, CancellationToken cancellationToken)
    {
        readDbContext.StoreProducts.Add(new StoreProductReadEntity
        {
            StoreLocationId = ev.StoreLocationId,
            ProductId = ev.ProductId,
            Stock = ev.Stock,
        });

        await readDbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Persisted product {ProductId} at store location {StoreLocationId} with stock {Stock}",
            ev.ProductId,
            ev.StoreLocationId,
            ev.Stock);
    }
}
