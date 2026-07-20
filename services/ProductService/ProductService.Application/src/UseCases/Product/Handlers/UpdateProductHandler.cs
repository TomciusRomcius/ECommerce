using ECommerceBackend.EventTypes;
using ECommerceBackend.Utils;
using EventSystemHelper.Kafka.Services;
using EventSystemHelper.Kafka.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProductService.Application.Persistence;
using ProductService.Application.UseCases.Product.Commands;

namespace ProductService.Application.UseCases.Product.Handlers;

public class UpdateProductHandler : IRequestHandler<UpdateProductCommand>
{
    private readonly DatabaseContext _context;
    private readonly ILogger<UpdateProductHandler> _logger;
    private readonly KafkaConfiguration _kafkaConfiguration;

    public UpdateProductHandler(
        ILogger<UpdateProductHandler> logger,
        DatabaseContext context,
        IOptions<KafkaConfiguration> kafkaConfiguration)
    {
        _context = context;
        _logger = logger;
        _kafkaConfiguration = kafkaConfiguration.Value;
    }

    public async Task Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        _logger.LogTrace("Entered Handle");
        _logger.LogDebug("Updating product with updator: {@Updator}", request.Updator);

        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.ProductId == request.Updator.ProductId, cancellationToken);

        if (product is null)
        {
            _logger.LogWarning("Failed to update product: product with id: {Id} does not exist", request.Updator.ProductId);
            return;
        }

        product.Name = request.Updator.Name ?? product.Name;
        product.Description = request.Updator.Description ?? product.Description;
        product.Price = request.Updator.Price ?? product.Price;
        product.ManufacturerId = request.Updator.ManufacturerId ?? product.ManufacturerId;
        product.CategoryId = request.Updator.CategoryId ?? product.CategoryId;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully updated product with id: {ProductId}", product.ProductId);

        var ev = new ProductUpdatedEvent
        {
            ProductId = product.ProductId,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            ManufacturerId = product.ManufacturerId,
            CategoryId = product.CategoryId,
        };

        string sEvent = JsonUtils.Serialize(ev);
        await new KafkaEventProducer(_kafkaConfiguration)
            .ProduceEventAsync(ev.TopicName, sEvent, cancellationToken);
    }
}
