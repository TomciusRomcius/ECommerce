using ApiWorker.Persistence;
using ECommerceBackend.EventTypes;
using Microsoft.EntityFrameworkCore;

namespace ApiWorker.Services;

public sealed class ProductDeletedHandler(
    ReadDbContext readDbContext,
    ILogger<ProductDeletedHandler> logger) : IEventHandler<ProductDeletedEvent>
{
    public async Task HandleAsync(ProductDeletedEvent ev, CancellationToken cancellationToken)
    {
        await readDbContext.ProductImages
            .Where(image => image.ProductId == ev.ProductId)
            .ExecuteDeleteAsync(cancellationToken);

        bool exists = await readDbContext.Products
            .AnyAsync(product => product.ProductId == ev.ProductId, cancellationToken);

        if (!exists)
        {
            logger.LogWarning("Product {ProductId} not found in read model for deletion", ev.ProductId);
            return;
        }

        await readDbContext.Products
            .Where(product => product.ProductId == ev.ProductId)
            .ExecuteDeleteAsync(cancellationToken);

        await readDbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Deleted product {ProductId} from read model", ev.ProductId);
    }
}
