using ApiWorker.Persistence;
using ECommerceBackend.EventTypes;
using Microsoft.EntityFrameworkCore;

namespace ApiWorker.Services;

public sealed class StoreUpdatedHandler(
    ReadDbContext readDbContext,
    ILogger<StoreUpdatedHandler> logger) : IEventHandler<StoreUpdatedEvent>
{
    public async Task HandleAsync(StoreUpdatedEvent ev, CancellationToken cancellationToken)
    {
        int rowsUpdated = await readDbContext.StoreLocations
            .Where(storeLocation => storeLocation.StoreLocationId == ev.StoreLocationId)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(storeLocation => storeLocation.DisplayName, ev.DisplayName)
                    .SetProperty(storeLocation => storeLocation.Address, ev.Address),
                cancellationToken);

        logger.LogInformation(
            "Updated store location {StoreLocationId} in read model; rows affected: {RowsUpdated}",
            ev.StoreLocationId,
            rowsUpdated);
    }
}
