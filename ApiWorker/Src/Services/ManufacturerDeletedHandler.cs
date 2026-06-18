using ApiWorker.Persistence;
using ECommerceBackend.EventTypes;
using Microsoft.EntityFrameworkCore;

namespace ApiWorker.Services;

public sealed class ManufacturerDeletedHandler(
    ReadDbContext readDbContext,
    ILogger<ManufacturerDeletedHandler> logger) : IEventHandler<ManufacturerDeletedEvent>
{
    public async Task HandleAsync(ManufacturerDeletedEvent ev, CancellationToken cancellationToken)
    {
        bool exists = await readDbContext.Manufacturers
            .AnyAsync(manufacturer => manufacturer.ManufacturerId == ev.ManufacturerId, cancellationToken);

        if (!exists)
        {
            logger.LogWarning(
                "Manufacturer {ManufacturerId} not found in read model for deletion",
                ev.ManufacturerId);
            return;
        }

        await readDbContext.Manufacturers
            .Where(manufacturer => manufacturer.ManufacturerId == ev.ManufacturerId)
            .ExecuteDeleteAsync(cancellationToken);

        await readDbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Deleted manufacturer {ManufacturerId} from read model", ev.ManufacturerId);
    }
}
