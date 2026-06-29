using ApiWorker.Persistence;
using ApiWorker.Persistence.Entities;
using ECommerceBackend.EventTypes;
using Microsoft.EntityFrameworkCore;

namespace ApiWorker.Services;

public sealed class ProductUpdatedHandler(
    ReadDbContext readDbContext,
    ILogger<ProductUpdatedHandler> logger) : IEventHandler<ProductUpdatedEvent>
{
    public async Task HandleAsync(ProductUpdatedEvent ev, CancellationToken cancellationToken)
    {
        ProductEntity? product = await readDbContext.Products
            .FindAsync([ev.ProductId], cancellationToken);

        if (product is null)
        {
            logger.LogWarning("Product {ProductId} not found in read model for update", ev.ProductId);
            return;
        }

        product.Name = ev.Name;
        product.Description = ev.Description;
        product.Price = ev.Price;
        product.ManufacturerId = ev.ManufacturerId;
        product.CategoryId = ev.CategoryId;

        await readDbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Updated product {ProductId} in read model", ev.ProductId);
    }
}
