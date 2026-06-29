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
        int storeProductsDeleted = await readDbContext.StoreProducts
            .Where(storeProduct => storeProduct.StoreLocationId == ev.StoreLocationId)
            .ExecuteDeleteAsync(cancellationToken);

        int storeLocationsDeleted = await readDbContext.StoreLocations
            .Where(storeLocation => storeLocation.StoreLocationId == ev.StoreLocationId)
            .ExecuteDeleteAsync(cancellationToken);

        logger.LogInformation(
            "Removed store location {StoreLocationId} from read model; deleted {StoreProductsDeleted} store products and {StoreLocationsDeleted} store location rows",
            ev.StoreLocationId,
            storeProductsDeleted,
            storeLocationsDeleted);
    }
}
