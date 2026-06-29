using ApiWorker.Persistence;
using ApiWorker.Persistence.Entities;
using ECommerceBackend.EventTypes;

namespace ApiWorker.Services;

public sealed class ProductImageCreatedHandler(
    ReadDbContext readDbContext,
    ILogger<ProductImageCreatedHandler> logger) : IEventHandler<ProductImageCreatedEvent>
{
    public async Task HandleAsync(ProductImageCreatedEvent ev, CancellationToken cancellationToken)
    {
        ProductImageReadEntity? existing = await readDbContext.ProductImages
            .FindAsync([ev.ProductImageId], cancellationToken);

        if (existing is null)
        {
            readDbContext.ProductImages.Add(new ProductImageReadEntity(
                ev.ProductImageId,
                ev.ProductId,
                ev.S3Key));
        }
        else
        {
            existing.ProductId = ev.ProductId;
            existing.S3Key = ev.S3Key;
        }

        await readDbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Upserted product image {ProductImageId} for product {ProductId} in read model",
            ev.ProductImageId,
            ev.ProductId);
    }
}
