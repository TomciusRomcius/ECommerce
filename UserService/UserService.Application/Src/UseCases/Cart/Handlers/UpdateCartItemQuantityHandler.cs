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

public class UpdateCartItemQuantityHandler : IRequestHandler<UpdateCartItemQuantityCommand, ResultError?>
{
    private readonly DatabaseContext _context;
    private readonly ILogger<UpdateCartItemQuantityHandler> _logger;
    private readonly KafkaConfiguration _kafkaConfiguration;

    public UpdateCartItemQuantityHandler(
        ILogger<UpdateCartItemQuantityHandler> logger,
        DatabaseContext context,
        IOptions<KafkaConfiguration> kafkaConfiguration)
    {
        _logger = logger;
        _context = context;
        _kafkaConfiguration = kafkaConfiguration.Value;
    }

    public async Task<ResultError?> Handle(UpdateCartItemQuantityCommand request, CancellationToken cancellationToken)
    {
        _logger.LogTrace("Entered Handle");
        _logger.LogDebug(
            "Updating cart product: {ProductId} quantity for user: {UserId} with new quantity: {Quantity}",
            request.CartProduct.ProductId,
            request.CartProduct.UserId,
            request.CartProduct.Quantity
        );
        int rowsAffected = await _context.CartProducts
            .Where(cp =>
                cp.UserId == request.CartProduct.UserId &&
                cp.ProductId == request.CartProduct.ProductId &&
                cp.StoreLocationId == request.CartProduct.StoreLocationId)
            .ExecuteUpdateAsync(setters =>
                setters.SetProperty(cp => cp.Quantity, request.CartProduct.Quantity), cancellationToken);

        if (rowsAffected == 0)
        {
            _logger.LogWarning(
                "Cart item not found for user {UserId}, product {ProductId}, store {StoreLocationId}",
                request.CartProduct.UserId,
                request.CartProduct.ProductId,
                request.CartProduct.StoreLocationId);
            return new ResultError(ResultErrorType.INVALID_OPERATION_ERROR, "Item not found in cart.");
        }

        _logger.LogInformation(
            "Updated cart product: {ProductId} quantity for user: {UserId} with new quantity: {Quantity}",
            request.CartProduct.ProductId,
            request.CartProduct.UserId,
            request.CartProduct.Quantity
        );

        var kafkaEvent = new ProductCartQuantityModifiedEvent
        {
            UserId = request.CartProduct.UserId,
            ProductId = request.CartProduct.ProductId,
            StoreLocationId = request.CartProduct.StoreLocationId,
            Quantity = request.CartProduct.Quantity,
        };
        string sEvent = JsonUtils.Serialize(kafkaEvent);
        await new KafkaEventProducer(_kafkaConfiguration)
            .ProduceEventAsync("product-cart-quantity-modified", sEvent, cancellationToken);

        return null;
    }
}
