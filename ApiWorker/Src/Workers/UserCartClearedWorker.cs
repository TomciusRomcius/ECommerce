using ApiWorker.Infrastructure;
using ECommerceBackend.EventTypes;
using EventSystemHelper.Kafka.Utils;
using Microsoft.Extensions.Options;

namespace ApiWorker.Workers;

public sealed class UserCartClearedWorker(
    ILogger<UserCartClearedWorker> logger,
    IOptions<KafkaConfiguration> kafkaConfiguration,
    IServiceScopeFactory serviceScopeFactory)
    : KafkaEventWorker<UserCartClearedEvent>(logger, kafkaConfiguration, serviceScopeFactory)
{
    protected override string Topic => "user_cart_cleared";
}
