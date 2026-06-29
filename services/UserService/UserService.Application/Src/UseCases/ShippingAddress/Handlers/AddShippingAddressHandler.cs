using MediatR;
using Microsoft.Extensions.Logging;
using UserService.Application.Persistence;
using UserService.Application.UseCases.ShippingAddress.Commands;

namespace UserService.Application.UseCases.ShippingAddress.Handlers;

public class AddShippingAddressHandler : IRequestHandler<AddShippingAddressCommand>
{
    private readonly DatabaseContext _context;
    private readonly ILogger<AddShippingAddressHandler> _logger;

    public AddShippingAddressHandler(ILogger<AddShippingAddressHandler> logger, DatabaseContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task Handle(AddShippingAddressCommand request, CancellationToken cancellationToken)
    {
        _logger.LogTrace("Entered Handle");
        _logger.LogDebug("Creating a shipping address: {@ShippingAddress}", request.Address);
        await _context.ShippingAddresses.AddAsync(request.Address, cancellationToken);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Succesfully created a shipping: {@ShippingAddress}", request.Address);
    }
}