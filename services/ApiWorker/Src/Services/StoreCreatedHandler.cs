using ApiWorker.Persistence;
using ApiWorker.Persistence.Entities;
using ECommerceBackend.EventTypes;

namespace ApiWorker.Services;

public sealed class StoreCreatedHandler(
    ReadDbContext readDbContext,
    ILogger<StoreCreatedHandler> logger) : IEventHandler<StoreCreatedEvent>
{
    public async Task HandleAsync(StoreCreatedEvent ev, CancellationToken cancellationToken)
    {
        readDbContext.StoreLocations.Add(new StoreLocationEntity(
            ev.StoreLocationId,
            ev.DisplayName,
            ev.Address));

        await readDbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Upserted store location {StoreLocationId} in read model",
            ev.StoreLocationId);
    }
}
