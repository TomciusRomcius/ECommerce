using ApiWorker;
using ApiWorker.Persistence;
using ApiWorker.Services;
using ApiWorker.Workers;
using ECommerceBackend.EventTypes;
using ECommerceBackend.Utils.Database;
using EventSystemHelper.Kafka.Utils;
using Microsoft.Extensions.Options;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddOptions<PostgresConfiguration>()
    .Bind(builder.Configuration.GetSection("Database"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

string? kafkaServers = builder.Configuration.GetSection("Kafka")["Servers"];
ArgumentException.ThrowIfNullOrWhiteSpace(kafkaServers);
builder.Services.AddSingleton<IOptions<KafkaConfiguration>>(
    Options.Create(new KafkaConfiguration(kafkaServers)));

builder.Services.AddDbContext<ReadDbContext>();
builder.Services.AddScoped<IEventHandler<ProductAddedToStoreEvent>, ProductAddedToStoreHandler>();
builder.Services.AddScoped<IEventHandler<ProductStockUpdatedEvent>, ProductStockUpdatedHandler>();
builder.Services.AddScoped<IEventHandler<ProductRemovedFromStoreEvent>, ProductRemovedFromStoreHandler>();
builder.Services.AddScoped<IEventHandler<ProductCreatedEvent>, ProductCreatedHandler>();
builder.Services.AddScoped<IEventHandler<ProductUpdatedEvent>, ProductUpdatedHandler>();
builder.Services.AddScoped<IEventHandler<ProductDeletedEvent>, ProductDeletedHandler>();
builder.Services.AddScoped<IEventHandler<StoreCreatedEvent>, StoreCreatedHandler>();
builder.Services.AddScoped<IEventHandler<StoreUpdatedEvent>, StoreUpdatedHandler>();
builder.Services.AddScoped<IEventHandler<StoreDeletedEvent>, StoreDeletedHandler>();
builder.Services.AddScoped<IEventHandler<ManufacturerCreatedEvent>, ManufacturerCreatedHandler>();
builder.Services.AddScoped<IEventHandler<ManufacturerUpdatedEvent>, ManufacturerUpdatedHandler>();
builder.Services.AddScoped<IEventHandler<ManufacturerDeletedEvent>, ManufacturerDeletedHandler>();
builder.Services.AddScoped<IEventHandler<CategoryCreatedEvent>, CategoryCreatedHandler>();
builder.Services.AddScoped<IEventHandler<CategoryUpdatedEvent>, CategoryUpdatedHandler>();
builder.Services.AddScoped<IEventHandler<CategoryDeletedEvent>, CategoryDeletedHandler>();
builder.Services.AddScoped<IEventHandler<ProductImageCreatedEvent>, ProductImageCreatedHandler>();
builder.Services.AddScoped<IEventHandler<ProductImageUpdatedEvent>, ProductImageUpdatedHandler>();
builder.Services.AddScoped<IEventHandler<ProductImageDeletedEvent>, ProductImageDeletedHandler>();

builder.Services.AddHostedService<ProductAddedToStoreWorker>();
builder.Services.AddHostedService<ProductStockUpdatedWorker>();
builder.Services.AddHostedService<ProductRemovedFromStoreWorker>();
builder.Services.AddHostedService<ProductCreatedWorker>();
builder.Services.AddHostedService<ProductUpdatedWorker>();
builder.Services.AddHostedService<ProductDeletedWorker>();
builder.Services.AddHostedService<StoreCreatedWorker>();
builder.Services.AddHostedService<StoreUpdatedWorker>();
builder.Services.AddHostedService<StoreDeletedWorker>();
builder.Services.AddHostedService<ManufacturerCreatedWorker>();
builder.Services.AddHostedService<ManufacturerUpdatedWorker>();
builder.Services.AddHostedService<ManufacturerDeletedWorker>();
builder.Services.AddHostedService<CategoryCreatedWorker>();
builder.Services.AddHostedService<CategoryUpdatedWorker>();
builder.Services.AddHostedService<CategoryDeletedWorker>();
builder.Services.AddHostedService<ProductImageCreatedWorker>();
builder.Services.AddHostedService<ProductImageUpdatedWorker>();
builder.Services.AddHostedService<ProductImageDeletedWorker>();

var host = builder.Build();
host.Run();
