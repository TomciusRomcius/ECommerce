using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserService.Application.Persistence;
using UserService.Application.UseCases.ShippingAddress.Commands;

namespace UserService.Application.UseCases.ShippingAddress.Handlers;

public class RemoveShippingAddressHandler : IRequestHandler<RemoveShippingAddressCommand>
{
    private readonly DatabaseContext _context;
    private readonly ILogger<RemoveShippingAddressHandler> _logger;

    public RemoveShippingAddressHandler(ILogger<RemoveShippingAddressHandler> logger, DatabaseContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task Handle(RemoveShippingAddressCommand request, CancellationToken cancellationToken)
    {
        _logger.LogTrace("Entered Handle");
        var userId = request.UserId.ToString();
        _logger.LogDebug("Removing shipping address of user: {UserId}", userId);
        int rowsAffected = await _context.ShippingAddresses
            .Where(sa => sa.UserId == userId)
            .ExecuteDeleteAsync(cancellationToken);
        if (rowsAffected > 0)
        {
            _logger.LogInformation("Deleted a shipping address of user: {@UserId}", userId);
        }
        else
        {
            _logger.LogWarning(@"Failed to delete the shipping address of user: {@UserId}.
                                The user does not exist or does not have an assosiated shipping address",
                userId);
        }
    }
}