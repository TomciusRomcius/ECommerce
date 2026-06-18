using ApiWorker.Persistence;
using ECommerceBackend.EventTypes;
using Microsoft.EntityFrameworkCore;

namespace ApiWorker.Services;

public sealed class StoreCreatedHandler(
    ReadDbContext readDbContext,
    ILogger<StoreCreatedHandler> logger) : IEventHandler<StoreCreatedEvent>
{
    public async Task HandleAsync(StoreCreatedEvent ev, CancellationToken cancellationToken)
    {
        int rowsUpdated = await readDbContext.StoreProducts
            .Where(storeProduct => storeProduct.StoreLocationId == ev.StoreLocationId)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(storeProduct => storeProduct.StoreDisplayName, ev.DisplayName)
                    .SetProperty(storeProduct => storeProduct.StoreAddress, ev.Address),
                cancellationToken);

        logger.LogInformation(
            "Store created event received for store location {StoreLocationId}; updated metadata on {RowsUpdated} store products",
            ev.StoreLocationId,
            rowsUpdated);
    }
}
