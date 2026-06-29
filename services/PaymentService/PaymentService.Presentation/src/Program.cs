using ECommerceBackend.Utils.Database;
using EventSystemHelper.Kafka.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PaymentService.Application.src.Interfaces;
using PaymentService.Application.src.Persistence;
using PaymentService.Application.src.Services;
using PaymentService.Domain.src.Enums;
using PaymentService.Infrastructure.src.Interfaces;
using PaymentService.Infrastructure.src.Services;
using PaymentService.Infrastructure.src.Utils;
using System.Reflection;
using PaymentService.Application.Services;
using PaymentService.Infrastructure.Interfaces;
using PaymentService.Infrastructure.Services;
using PaymentService.Presentation.BackgroundServices;

// TODO: separate initialization logic
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging();
builder.Services.AddControllers();
//builder.Services.AddOpenApi();

builder.Services.AddOptions<StripeSettings>()
    .Bind(builder.Configuration.GetSection("Stripe"))
    .ValidateDataAnnotations()
    .ValidateOnStart();


builder.Services.AddOptions<PostgresConfiguration>()
    .Bind(builder.Configuration.GetSection("Database"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddDbContext<DatabaseContext>();

string? kafkaServers = builder.Configuration.GetSection("Kafka")["Servers"];
if (String.IsNullOrWhiteSpace(kafkaServers))
{
    throw new InvalidDataException("Kafka__Servers not defined");
}

// Cant currently use normal AddOptions as KafkaConfiguration does not have a default constructor
builder.Services.AddSingleton(Options.Create(new KafkaConfiguration(
    kafkaServers
)));
builder.Services.AddSingleton<WebhookEventStrategyMapContainer>();
builder.Services.AddTransient<IWebhookCoordinatorService, StripeWebhookCoordinatorService>();
builder.Services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
builder.Services.AddTransient<IPaymentSessionFactory, PaymentSessionFactory>();
builder.Services.AddTransient<IPaymentSessionPersistenceService, PaymentSessionPersistenceService>();
builder.Services.AddTransient<IPaymentSessionCoordinator, PaymentSessionCoordinator>();
builder.Services.AddScoped<IChargeSucceededConsumer, ChargeSucceededConsumer>();
builder.Services.AddHostedService<ChargeSucceededBackgroundService>();

// Get all stripe webhook event handling strategies and register them to the DI container
Assembly? infrastructureAssembly = Assembly.GetAssembly(typeof(IStripeWebhookStrategy));
ArgumentNullException.ThrowIfNull(infrastructureAssembly);
IEnumerable<Type> validTypes = infrastructureAssembly.GetTypes()
    .Where(t => typeof(IStripeWebhookStrategy)
    .IsAssignableFrom(t)
    && // Only retrieve classes
    t is { IsClass: true, IsAbstract: false });

foreach (Type type in infrastructureAssembly.GetTypes().Where(t => typeof(IStripeWebhookStrategy).IsAssignableFrom(t) &&
    t is { IsClass: true, IsAbstract: false }))
{
    builder.Services.AddSingleton(typeof(IStripeWebhookStrategy), type);
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
}

using (var scope = app.Services.CreateScope())
{
    // Wait for database context to establish a database connection and for 
    // migrator service to apply migrations 
    int maxTries = 12;
    int timeoutSeconds = 10;
    var ctx = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
    for (int i = 0; i < maxTries; i++)
    {
        if (ctx.Database.CanConnect() && !ctx.Database.GetPendingMigrations().Any())
        {
            break;
        }
        Task.Delay(TimeSpan.FromSeconds(timeoutSeconds)).Wait();
    }

    WebhookEventStrategyMapContainer c = scope.ServiceProvider.GetRequiredService<WebhookEventStrategyMapContainer>();
    WebhookEventStrategyMap<IStripeWebhookStrategy> stripeStrategyMap = new WebhookEventStrategyMap<IStripeWebhookStrategy>();

    // Populate the stripe webhook strategy map with registered strategies
    var stripeStrategies = scope.ServiceProvider.GetServices<IStripeWebhookStrategy>();
    foreach (var strategy in stripeStrategies)
    {
        stripeStrategyMap.AddStrategy(strategy.EventType, strategy);
    }

    // Add strategy map to the strategy map container
    c.AddStrategyMap(PaymentProvider.STRIPE, stripeStrategyMap);
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
