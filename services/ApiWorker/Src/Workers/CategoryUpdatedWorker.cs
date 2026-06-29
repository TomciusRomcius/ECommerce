using ApiWorker.Infrastructure;
using ECommerceBackend.EventTypes;
using EventSystemHelper.Kafka.Utils;
using Microsoft.Extensions.Options;

namespace ApiWorker.Workers;

public sealed class CategoryUpdatedWorker(
    ILogger<CategoryUpdatedWorker> logger,
    IOptions<KafkaConfiguration> kafkaConfiguration,
    IServiceScopeFactory serviceScopeFactory)
    : KafkaEventWorker<CategoryUpdatedEvent>(logger, kafkaConfiguration, serviceScopeFactory)
{
    protected override string Topic => "category_updated";

}
