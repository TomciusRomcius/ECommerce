using ApiWorker.Infrastructure;
using ECommerceBackend.EventTypes;
using EventSystemHelper.Kafka.Utils;
using Microsoft.Extensions.Options;

namespace ApiWorker.Workers;

public sealed class StoreDeletedWorker(
    ILogger<StoreDeletedWorker> logger,
    IOptions<KafkaConfiguration> kafkaConfiguration,
    IServiceScopeFactory serviceScopeFactory)
    : KafkaEventWorker<StoreDeletedEvent>(logger, kafkaConfiguration, serviceScopeFactory)
{
    protected override string Topic => "store-deleted";

    protected override string EventLabel => "Store-deleted";
}
