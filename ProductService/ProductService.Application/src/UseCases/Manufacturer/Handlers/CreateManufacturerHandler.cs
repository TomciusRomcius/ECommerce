using ECommerceBackend.EventTypes;
using ECommerceBackend.Utils;
using EventSystemHelper.Kafka.Services;
using EventSystemHelper.Kafka.Utils;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProductService.Application.Persistence;
using ProductService.Application.UseCases.Manufacturer.Commands;
using ProductService.Domain.Entities;
using ProductService.Domain.Utils;

namespace ProductService.Application.UseCases.Manufacturer.Handlers;

public class CreateManufacturerHandler : IRequestHandler<CreateManufacturerCommand, Result<int>>
{
    private readonly DatabaseContext _context;
    private readonly ILogger<CreateManufacturerHandler> _logger;
    private readonly KafkaConfiguration _kafkaConfiguration;

    public CreateManufacturerHandler(
        ILogger<CreateManufacturerHandler> logger,
        DatabaseContext context,
        IOptions<KafkaConfiguration> kafkaConfiguration)
    {
        _logger = logger;
        _context = context;
        _kafkaConfiguration = kafkaConfiguration.Value;
    }

    public async Task<Result<int>> Handle(CreateManufacturerCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogTrace("Entered Handle");
        var entity = new ManufacturerEntity(request.Name);
        _logger.LogDebug("Creating manufacturer: {@Manufacturer}", entity);

        await _context.Manufacturers.AddAsync(entity, cancellationToken);

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An exception was thrown while persisting to the database.");

            return new Result<int>([
                new ResultError(
                    ResultErrorType.UNKNOWN_ERROR,
                    "Failed to create manufacturer"
                )
            ]);
        }

        _logger.LogDebug("Created manufacturer with id: {Id}", entity.ManufacturerId);

        var ev = new ManufacturerCreatedEvent
        {
            ManufacturerId = entity.ManufacturerId,
            Name = entity.Name,
        };

        string sEvent = JsonUtils.Serialize(ev);
        await new KafkaEventProducer(_kafkaConfiguration)
            .ProduceEventAsync("manufacturer-created", sEvent, cancellationToken);

        return new Result<int>(entity.ManufacturerId);
    }
}
