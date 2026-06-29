using ApiWorker.Persistence;
using ECommerceBackend.EventTypes;
using Microsoft.EntityFrameworkCore;

namespace ApiWorker.Services;

public sealed class ManufacturerUpdatedHandler(
    ReadDbContext readDbContext,
    ILogger<ManufacturerUpdatedHandler> logger) : IEventHandler<ManufacturerUpdatedEvent>
{
    public async Task HandleAsync(ManufacturerUpdatedEvent ev, CancellationToken cancellationToken)
    {
        int rowsAffected = await readDbContext.Manufacturers
            .Where(manufacturer => manufacturer.ManufacturerId == ev.ManufacturerId)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(manufacturer => manufacturer.Name, ev.Name),
                cancellationToken);

        if (rowsAffected == 0)
        {
            logger.LogWarning(
                "Manufacturer {ManufacturerId} not found in read model for update",
                ev.ManufacturerId);
            return;
        }

        await readDbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Updated manufacturer {ManufacturerId} in read model", ev.ManufacturerId);
    }
}
