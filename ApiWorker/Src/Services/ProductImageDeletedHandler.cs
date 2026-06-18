using ApiWorker.Persistence;
using ECommerceBackend.EventTypes;
using Microsoft.EntityFrameworkCore;

namespace ApiWorker.Services;

public sealed class ProductImageDeletedHandler(
    ReadDbContext readDbContext,
    ILogger<ProductImageDeletedHandler> logger) : IEventHandler<ProductImageDeletedEvent>
{
    public async Task HandleAsync(ProductImageDeletedEvent ev, CancellationToken cancellationToken)
    {
        bool exists = await readDbContext.ProductImages
            .AnyAsync(image => image.ProductImageId == ev.ProductImageId, cancellationToken);

        if (!exists)
        {
            logger.LogWarning(
                "Product image {ProductImageId} not found in read model for deletion",
                ev.ProductImageId);
            return;
        }

        await readDbContext.ProductImages
            .Where(image => image.ProductImageId == ev.ProductImageId)
            .ExecuteDeleteAsync(cancellationToken);

        logger.LogInformation("Deleted product image {ProductImageId} from read model", ev.ProductImageId);
    }
}
