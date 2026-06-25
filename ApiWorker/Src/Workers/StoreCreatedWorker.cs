using ApiWorker.Infrastructure;
using ECommerceBackend.EventTypes;
using EventSystemHelper.Kafka.Utils;
using Microsoft.Extensions.Options;

namespace ApiWorker.Workers;

public sealed class StoreCreatedWorker(
    ILogger<StoreCreatedWorker> logger,
    IOptions<KafkaConfiguration> kafkaConfiguration,
    IServiceScopeFactory serviceScopeFactory)
    : KafkaEventWorker<StoreCreatedEvent>(logger, kafkaConfiguration, serviceScopeFactory)
{
    protected override string Topic => "store_created";

}
