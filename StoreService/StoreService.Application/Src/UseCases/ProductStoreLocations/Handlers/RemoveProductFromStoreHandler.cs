using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StoreService.Application.Persistence;
using StoreService.Application.UseCases.ProductStoreLocations.Commands;

namespace StoreService.Application.UseCases.ProductStoreLocations.Handlers;

public class RemoveProductFromStoreHandler : IRequestHandler<RemoveProductFromStoreCommand>
{
    private readonly DatabaseContext _context;
    private readonly ILogger<RemoveProductFromStoreHandler> _logger;

    public RemoveProductFromStoreHandler(ILogger<RemoveProductFromStoreHandler> logger, DatabaseContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task Handle(RemoveProductFromStoreCommand request, CancellationToken cancellationToken)
    {
        _logger.LogTrace("Entered Handle");
        var rowsAffected = await _context.ProductStoreLocations
            .Where(psl => psl.StoreLocationId == request.StoreLocationId && psl.ProductId == request.ProductId)
            .ExecuteDeleteAsync(cancellationToken);

        if (rowsAffected > 0)
            _logger.LogInformation(
                "Removed product(id: {ProductId}) from store location(id: {StoreLocationId})",
                request.ProductId,
                request.StoreLocationId
            );
        else
            _logger.LogWarning(@"Failed to remove product(id: {ProductId}) from store location(id: {StoreLocationId})
                                because product or store location does not exist",
                request.ProductId,
                request.StoreLocationId
            );
    }
}