using ApiWorker.Persistence;
using ApiWorker.Persistence.Entities;
using ECommerceBackend.EventTypes;

namespace ApiWorker.Services;

public sealed class ProductImageUpdatedHandler(
    ReadDbContext readDbContext,
    ILogger<ProductImageUpdatedHandler> logger) : IEventHandler<ProductImageUpdatedEvent>
{
    public async Task HandleAsync(ProductImageUpdatedEvent ev, CancellationToken cancellationToken)
    {
        ProductImageReadEntity? image = await readDbContext.ProductImages
            .FindAsync([ev.ProductImageId], cancellationToken);

        if (image is null)
        {
            logger.LogWarning(
                "Product image {ProductImageId} not found in read model for update",
                ev.ProductImageId);
            return;
        }

        image.ProductId = ev.ProductId;
        image.S3Key = ev.S3Key;

        await readDbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Updated product image {ProductImageId} in read model", ev.ProductImageId);
    }
}
