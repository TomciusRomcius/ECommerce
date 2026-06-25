using AutoMapper;
using ECommerceBackend.EventTypes;
using ECommerceBackend.Utils;
using EventSystemHelper.Kafka.Services;
using EventSystemHelper.Kafka.Utils;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StoreService.Application.Persistence;
using StoreService.Application.UseCases.StoreLocation.Commands;
using StoreService.Domain.Entities;

namespace StoreService.Application.UseCases.StoreLocation.Handlers;

public class CreateStoreLocationHandler : IRequestHandler<CreateStoreLocationCommand, StoreLocationEntity?>
{
    private readonly DatabaseContext _context;
    private readonly ILogger<CreateStoreLocationHandler> _logger;
    private readonly IMapper _mapper;
    private readonly KafkaConfiguration _kafkaConfiguration;

    public CreateStoreLocationHandler(
        DatabaseContext context,
        ILogger<CreateStoreLocationHandler> logger,
        IMapper mapper,
        IOptions<KafkaConfiguration> kafkaConfiguration)
    {
        _context = context;
        _logger = logger;
        _mapper = mapper;
        _kafkaConfiguration = kafkaConfiguration.Value;
    }

    public async Task<StoreLocationEntity?> Handle(CreateStoreLocationCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogTrace(
            "Handling CreateStoreLocationCommand for DisplayName: {DisplayName}, Address: {Address}",
            request.StoreLocation.DisplayName,
            request.StoreLocation.Address
        );

        var entity = _mapper.Map<StoreLocationEntity>(request.StoreLocation);

        _logger.LogDebug("Creating StoreLocationEntity instance: {@Entity}", entity);

        await _context.StoreLocations.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogDebug("StoreLocationEntity persisted with StoreLocationId: {StoreLocationId}",
            entity.StoreLocationId);

        var ev = new StoreCreatedEvent
        {
            StoreLocationId = entity.StoreLocationId,
            DisplayName = entity.DisplayName,
            Address = entity.Address,
        };

        string sEvent = JsonUtils.Serialize(ev);
        await new KafkaEventProducer(_kafkaConfiguration)
            .ProduceEventAsync(ev.TopicName, sEvent, cancellationToken);

        return entity;
    }
}
