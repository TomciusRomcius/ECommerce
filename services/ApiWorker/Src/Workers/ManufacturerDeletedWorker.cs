using ApiWorker.Infrastructure;
using ECommerceBackend.EventTypes;
using EventSystemHelper.Kafka.Utils;
using Microsoft.Extensions.Options;

namespace ApiWorker.Workers;

public sealed class ManufacturerDeletedWorker(
    ILogger<ManufacturerDeletedWorker> logger,
    IOptions<KafkaConfiguration> kafkaConfiguration,
    IServiceScopeFactory serviceScopeFactory)
    : KafkaEventWorker<ManufacturerDeletedEvent>(logger, kafkaConfiguration, serviceScopeFactory)
{
    protected override string Topic => "manufacturer_deleted";

}
