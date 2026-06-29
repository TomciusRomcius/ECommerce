using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StoreService.Application.Persistence;
using StoreService.Application.UseCases.StoreLocation.Queries;
using StoreService.Domain.Entities;

namespace StoreService.Application.UseCases.StoreLocation.Handlers;

public class GetStoreLocationsHandler : IRequestHandler<GetStoreLocationsQuery, List<StoreLocationEntity>>
{
    private readonly DatabaseContext _context;
    private readonly ILogger<GetStoreLocationsHandler> _logger;

    public GetStoreLocationsHandler(ILogger<GetStoreLocationsHandler> logger, DatabaseContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<List<StoreLocationEntity>> Handle(GetStoreLocationsQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogTrace("Entered Handle");
        _logger.LogDebug(
            "Fetching store locations, page number: {PageNumber} page size: {PageSize}",
            request.PageNumber,
            DatabaseContext.PageSize
        );
        List<StoreLocationEntity> result = await _context.StoreLocations.ToListAsync(cancellationToken);
        _logger.LogDebug("Retrieved store locations: {@StoreLocations}", result);
        return result;
    }
}