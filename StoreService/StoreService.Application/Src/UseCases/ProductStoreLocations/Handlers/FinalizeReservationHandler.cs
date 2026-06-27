using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StoreService.Application.Persistence;
using StoreService.Application.UseCases.ProductStoreLocations.Commands;

namespace StoreService.Application.UseCases.ProductStoreLocations.Handlers;

public class FinalizeReservationHandler(
    ILogger<FinalizeReservationHandler> logger,
    DatabaseContext context) : IRequestHandler<FinalizeReservationCommand>
{
    public async Task Handle(FinalizeReservationCommand request, CancellationToken cancellationToken)
    {
        logger.LogTrace("Entered Handle");
        logger.LogDebug("Finalizing reservation for order {OrderId}", request.OrderId);

        await context.ReservedProducts
            .Where(rp => rp.OrderId == request.OrderId)
            .ExecuteDeleteAsync(cancellationToken);

        logger.LogInformation("Removed reservation records for order {OrderId}", request.OrderId);
    }
}
