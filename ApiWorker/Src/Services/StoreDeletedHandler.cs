using ApiWorker.Persistence;
using ECommerceBackend.EventTypes;
using Microsoft.EntityFrameworkCore;

namespace ApiWorker.Services;

public sealed class StoreDeletedHandler(
    ReadDbContext readDbContext,
    ILogger<StoreDeletedHandler> logger) : IEventHandler<StoreDeletedEvent>
{
    public async Task HandleAsync(StoreDeletedEvent ev, CancellationToken cancellationToken)
    {
        int rowsDeleted = await readDbContext.StoreProducts
            .Where(storeProduct => storeProduct.StoreLocationId == ev.StoreLocationId)
            .ExecuteDeleteAsync(cancellationToken);

        logger.LogInformation(
            "Removed {RowsDeleted} store products for deleted store location {StoreLocationId}",
            rowsDeleted,
            ev.StoreLocationId);
    }
}
