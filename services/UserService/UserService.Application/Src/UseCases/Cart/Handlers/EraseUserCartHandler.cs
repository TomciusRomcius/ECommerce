using ECommerceBackend.EventTypes;
using ECommerceBackend.Utils;
using EventSystemHelper.Kafka.Services;
using EventSystemHelper.Kafka.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UserService.Application.Persistence;
using UserService.Application.UseCases.Cart.Commands;

namespace UserService.Application.UseCases.Cart.Handlers;

public class EraseUserCartHandler : IRequestHandler<EraseUserCartCommand>
{
    private readonly ILogger<EraseUserCartHandler> _logger;
    private readonly DatabaseContext _context;
    private readonly KafkaConfiguration _kafkaConfiguration;

    public EraseUserCartHandler(ILogger<EraseUserCartHandler> logger, DatabaseContext context,
        IOptions<KafkaConfiguration> kafkaConfiguration)
    {
        _logger = logger;
        _context = context;
        _kafkaConfiguration = kafkaConfiguration.Value;
    }

    public async Task Handle(EraseUserCartCommand request, CancellationToken cancellationToken)
    {
        _logger.LogTrace("Entered Handle");
        _logger.LogDebug("Erasing user's(id: {UserId}) cart", request.UserId);
        var userId = request.UserId.ToString();
        int rowsAffected = await _context.CartProducts
            .Where(cp => cp.UserId == userId)
            .ExecuteDeleteAsync(cancellationToken);

        if (rowsAffected <= 0)
        {
            _logger.LogWarning(@"Failed to delete erase cart items of user: {@UserId}.
                                The user does not exist or does not have any cart items",
                userId);
            return;
        }

        _logger.LogInformation("Erasing cart items of user: {@UserId}", userId);
        var producer = new KafkaEventProducer(_kafkaConfiguration);
        var ev = new UserCartClearedEvent { UserId = userId };
        var sEvent = JsonUtils.Serialize(ev);
        await producer.ProduceEventAsync(ev.TopicName, sEvent, cancellationToken);
    }
}