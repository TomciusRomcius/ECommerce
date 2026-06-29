using ECommerceBackend.EventTypes;
using ECommerceBackend.Utils;
using EventSystemHelper.Kafka.Services;
using EventSystemHelper.Kafka.Utils;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UserService.Application.Persistence;
using UserService.Application.UseCases.Cart.Commands;
using UserService.Domain.Utils;

namespace UserService.Application.UseCases.Cart.Handlers;

public class AddItemToCartHandler : IRequestHandler<AddItemToCartCommand, ResultError?>
{
    private readonly DatabaseContext _context;
    private readonly ILogger<AddItemToCartHandler> _logger;
    private readonly KafkaConfiguration _kafkaConfiguration;

    public AddItemToCartHandler(
        ILogger<AddItemToCartHandler> logger,
        DatabaseContext context,
        IOptions<KafkaConfiguration> kafkaConfiguration)
    {
        _logger = logger;
        _context = context;
        _kafkaConfiguration = kafkaConfiguration.Value;
    }

    public async Task<ResultError?> Handle(AddItemToCartCommand request, CancellationToken cancellationToken)
    {
        _logger.LogTrace("Entered Handle");
        _logger.LogDebug(
            "Adding product {ProductId} to user's(id: {UserId}) cart"
            , request.CartProduct.ProductId,
            request.CartProduct.UserId
        );

        await _context.AddAsync(request.CartProduct, cancellationToken);

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, "An exception was thrown while persisting cart item to the database.");
            return new ResultError(ResultErrorType.UNKNOWN_ERROR, "Failed to add the item to the cart.");
        }

        var kafkaEvent = new ProductAddedToCartEvent
        {
            UserId = request.CartProduct.UserId,
            ProductId = request.CartProduct.ProductId,
            StoreLocationId = request.CartProduct.StoreLocationId,
            Quantity = request.CartProduct.Quantity,
        };
        string sEvent = JsonUtils.Serialize(kafkaEvent);
        await new KafkaEventProducer(_kafkaConfiguration)
            .ProduceEventAsync(kafkaEvent.TopicName, sEvent, cancellationToken);

        _logger.LogInformation(
            "Succesfully added product {ProductId} to user's(id: {UserId}) cart",
            request.CartProduct.ProductId,
            request.CartProduct.UserId
        );
        return null;
    }
}
