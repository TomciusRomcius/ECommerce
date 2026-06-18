using ApiWorker.Infrastructure;
using ECommerceBackend.EventTypes;
using EventSystemHelper.Kafka.Utils;
using Microsoft.Extensions.Options;

namespace ApiWorker.Workers;

public sealed class CategoryCreatedWorker(
    ILogger<CategoryCreatedWorker> logger,
    IOptions<KafkaConfiguration> kafkaConfiguration,
    IServiceScopeFactory serviceScopeFactory)
    : KafkaEventWorker<CategoryCreatedEvent>(logger, kafkaConfiguration, serviceScopeFactory)
{
    protected override string Topic => "category-created";

    protected override string EventLabel => "Category-created";
}
