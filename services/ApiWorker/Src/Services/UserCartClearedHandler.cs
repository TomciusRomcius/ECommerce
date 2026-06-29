using ApiWorker.Persistence;
using ECommerceBackend.EventTypes;
using Microsoft.EntityFrameworkCore;

namespace ApiWorker.Services;

public sealed class UserCartClearedHandler(
    ReadDbContext readDbContext,
    ILogger<UserCartClearedHandler> logger) : IEventHandler<UserCartClearedEvent>
{
    public async Task HandleAsync(UserCartClearedEvent ev, CancellationToken cancellationToken)
    {
        await readDbContext.CartProducts
            .Where(cp => cp.UserId == ev.UserId)
            .ExecuteDeleteAsync(cancellationToken);
        await readDbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Erased cart of user {UserId} in read model", ev.UserId);
    }
}
