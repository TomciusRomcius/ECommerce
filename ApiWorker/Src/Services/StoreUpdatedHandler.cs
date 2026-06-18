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
        int rowsUpdated = await readDbContext.StoreProducts
            .Where(storeProduct => storeProduct.StoreLocationId == ev.StoreLocationId)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(storeProduct => storeProduct.StoreDisplayName, ev.DisplayName)
                    .SetProperty(storeProduct => storeProduct.StoreAddress, ev.Address),
                cancellationToken);

        logger.LogInformation(
            "Updated store metadata on {RowsUpdated} store products for store location {StoreLocationId}",
            rowsUpdated,
            ev.StoreLocationId);
    }
}
