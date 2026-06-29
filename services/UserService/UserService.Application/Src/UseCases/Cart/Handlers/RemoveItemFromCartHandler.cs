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
using UserService.Domain.Utils;

namespace UserService.Application.UseCases.Cart.Handlers;

public class RemoveItemFromCartHandler : IRequestHandler<RemoveItemFromCartCommand, ResultError?>
{
    private readonly DatabaseContext _context;
    private readonly ILogger<RemoveItemFromCartHandler> _logger;
    private readonly KafkaConfiguration _kafkaConfiguration;

    public RemoveItemFromCartHandler(
        ILogger<RemoveItemFromCartHandler> logger,
        DatabaseContext context,
        IOptions<KafkaConfiguration> kafkaConfiguration)
    {
        _logger = logger;
        _context = context;
        _kafkaConfiguration = kafkaConfiguration.Value;
    }

    public async Task<ResultError?> Handle(RemoveItemFromCartCommand request, CancellationToken cancellationToken)
    {
        _logger.LogTrace("Entered Handle");
        _logger.LogDebug(
            "Removing product {ProductId} from cart for user {UserId} at store {StoreLocationId}",
            request.ProductId,
            request.UserId,
            request.StoreLocationId);

        int rowsAffected = await _context.CartProducts
            .Where(cp =>
                cp.UserId == request.UserId &&
                cp.ProductId == request.ProductId &&
                cp.StoreLocationId == request.StoreLocationId)
            .ExecuteDeleteAsync(cancellationToken);

        if (rowsAffected == 0)
        {
            _logger.LogWarning(
                "Cart item not found for user {UserId}, product {ProductId}, store {StoreLocationId}",
                request.UserId,
                request.ProductId,
                request.StoreLocationId);
            return new ResultError(ResultErrorType.INVALID_OPERATION_ERROR, "Item not found in cart.");
        }

        var kafkaEvent = new ProductRemovedFromCartEvent
        {
            UserId = request.UserId,
            ProductId = request.ProductId,
            StoreLocationId = request.StoreLocationId,
        };
        string sEvent = JsonUtils.Serialize(kafkaEvent);
        await new KafkaEventProducer(_kafkaConfiguration)
            .ProduceEventAsync(kafkaEvent.TopicName, sEvent, cancellationToken);

        return null;
    }
}
