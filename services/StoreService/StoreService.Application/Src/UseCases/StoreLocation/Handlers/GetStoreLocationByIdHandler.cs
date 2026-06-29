using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StoreService.Application.Persistence;
using StoreService.Application.UseCases.StoreLocation.Queries;
using StoreService.Domain.Entities;

namespace StoreService.Application.UseCases.StoreLocation.Handlers;

public class GetStoreLocationByIdHandler : IRequestHandler<GetStoreLocationByIdQuery, StoreLocationEntity?>
{
    private readonly DatabaseContext _context;
    private readonly ILogger<GetStoreLocationByIdHandler> _logger;

    public GetStoreLocationByIdHandler(ILogger<GetStoreLocationByIdHandler> logger, DatabaseContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<StoreLocationEntity?> Handle(
        GetStoreLocationByIdQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogTrace("Entered Handle");
        _logger.LogDebug("Fetching store location with id: {StoreLocationId}", request.StoreLocationId);

        StoreLocationEntity? result = await _context.StoreLocations
            .AsNoTracking()
            .FirstOrDefaultAsync(sl => sl.StoreLocationId == request.StoreLocationId, cancellationToken);

        if (result is null)
        {
            _logger.LogWarning("Store location not found with id: {StoreLocationId}", request.StoreLocationId);
        }

        return result;
    }
}
