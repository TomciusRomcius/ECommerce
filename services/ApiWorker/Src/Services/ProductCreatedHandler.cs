using ApiWorker.Persistence;
using ApiWorker.Persistence.Entities;
using ECommerceBackend.EventTypes;

namespace ApiWorker.Services;

public sealed class ProductCreatedHandler(
    ReadDbContext readDbContext,
    ILogger<ProductCreatedHandler> logger) : IEventHandler<ProductCreatedEvent>
{
    public async Task HandleAsync(ProductCreatedEvent ev, CancellationToken cancellationToken)
    {
        ProductEntity? existing = await readDbContext.Products
            .FindAsync([ev.ProductId], cancellationToken);

        if (existing is null)
        {
            readDbContext.Products.Add(new ProductEntity(
                ev.ProductId,
                ev.Name,
                ev.Description,
                ev.Price,
                ev.ManufacturerId,
                ev.CategoryId));
        }
        else
        {
            existing.Name = ev.Name;
            existing.Description = ev.Description;
            existing.Price = ev.Price;
            existing.ManufacturerId = ev.ManufacturerId;
            existing.CategoryId = ev.CategoryId;
        }

        await readDbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Upserted product {ProductId} in read model", ev.ProductId);
    }
}
