using ApiWorker.Infrastructure;
using ECommerceBackend.EventTypes;
using EventSystemHelper.Kafka.Utils;
using Microsoft.Extensions.Options;

namespace ApiWorker.Workers;

public sealed class ManufacturerCreatedWorker(
    ILogger<ManufacturerCreatedWorker> logger,
    IOptions<KafkaConfiguration> kafkaConfiguration,
    IServiceScopeFactory serviceScopeFactory)
    : KafkaEventWorker<ManufacturerCreatedEvent>(logger, kafkaConfiguration, serviceScopeFactory)
{
    protected override string Topic => "manufacturer-created";

    protected override string EventLabel => "Manufacturer-created";
}
