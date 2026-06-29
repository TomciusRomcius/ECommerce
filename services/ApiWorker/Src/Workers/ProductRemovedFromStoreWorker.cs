using ApiWorker.Infrastructure;
using ECommerceBackend.EventTypes;
using EventSystemHelper.Kafka.Utils;
using Microsoft.Extensions.Options;

namespace ApiWorker.Workers;

public sealed class ProductRemovedFromStoreWorker(
    ILogger<ProductRemovedFromStoreWorker> logger,
    IOptions<KafkaConfiguration> kafkaConfiguration,
    IServiceScopeFactory serviceScopeFactory)
    : KafkaEventWorker<ProductRemovedFromStoreEvent>(logger, kafkaConfiguration, serviceScopeFactory)
{
    protected override string Topic => "product_removed_from_store";

}
