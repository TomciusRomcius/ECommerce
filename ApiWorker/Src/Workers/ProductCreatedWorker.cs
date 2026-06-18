using ApiWorker.Infrastructure;
using ECommerceBackend.EventTypes;
using EventSystemHelper.Kafka.Utils;
using Microsoft.Extensions.Options;

namespace ApiWorker.Workers;

public sealed class ProductCreatedWorker(
    ILogger<ProductCreatedWorker> logger,
    IOptions<KafkaConfiguration> kafkaConfiguration,
    IServiceScopeFactory serviceScopeFactory)
    : KafkaEventWorker<ProductCreatedEvent>(logger, kafkaConfiguration, serviceScopeFactory)
{
    protected override string Topic => "product-created";

    protected override string EventLabel => "Product-created";
}
