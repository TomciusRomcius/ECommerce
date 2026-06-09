using ApiWorker;
using ApiWorker.Persistence;
using ApiWorker.Services;
using ECommerceBackend.Utils.Database;
using ECommerceBackend.Utils.Microservices;
using EventSystemHelper.Kafka.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddOptions<PostgresConfiguration>()
    .Bind(builder.Configuration.GetSection("Database"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddOptions<MicroserviceHosts>()
    .Bind(builder.Configuration.GetSection("MicroserviceNetworkConfig"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

string? kafkaServers = builder.Configuration.GetSection("Kafka")["Servers"];
ArgumentException.ThrowIfNullOrWhiteSpace(kafkaServers);
builder.Services.AddSingleton<IOptions<KafkaConfiguration>>(
    Options.Create(new KafkaConfiguration(kafkaServers)));

builder.Services.AddHttpClient();
builder.Services.AddDbContext<ReadDbContext>();
builder.Services.AddScoped<IProductAddedToStoreHandler, ProductAddedToStoreHandler>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
