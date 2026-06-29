using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserService.Application.Persistence;
using UserService.Application.UseCases.ShippingAddress.Queries;
using UserService.Domain.Entities;

namespace UserService.Application.UseCases.ShippingAddress.Handlers;

public class GetShippingAddressesHandler : IRequestHandler<GetShippingAddressesQuery, List<ShippingAddressEntity>>
{
    private readonly DatabaseContext _context;
    private readonly ILogger<GetShippingAddressesHandler> _logger;

    public GetShippingAddressesHandler(ILogger<GetShippingAddressesHandler> logger, DatabaseContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<List<ShippingAddressEntity>> Handle(GetShippingAddressesQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogTrace("Entered Handle");
        var userId = request.UserId.ToString();
        _logger.LogDebug("Getting shipping address for user: {UserId}", userId);

        List<ShippingAddressEntity> result = await _context.ShippingAddresses
            .Where(sa => sa.UserId == userId)
            .ToListAsync(cancellationToken: cancellationToken);

        _logger.LogDebug("Retrieved addresses: {@Addresses}", result);
        return result;
    }
}