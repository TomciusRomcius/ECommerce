using ApiWorker.Persistence;
using ApiWorker.Persistence.Entities;
using ECommerceBackend.EventTypes;

namespace ApiWorker.Services;

public sealed class ManufacturerCreatedHandler(
    ReadDbContext readDbContext,
    ILogger<ManufacturerCreatedHandler> logger) : IEventHandler<ManufacturerCreatedEvent>
{
    public async Task HandleAsync(ManufacturerCreatedEvent ev, CancellationToken cancellationToken)
    {
        readDbContext.Manufacturers.Add(new ManufacturerEntity(ev.ManufacturerId, ev.Name));
        await readDbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Upserted manufacturer {ManufacturerId} in read model", ev.ManufacturerId);
    }
}
