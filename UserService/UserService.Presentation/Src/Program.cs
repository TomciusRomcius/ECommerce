using ECommerceBackend.Utils.Auth;
using ECommerceBackend.Utils.Database;
using EventSystemHelper.Kafka.Utils;
using Microsoft.Extensions.Options;
using UserService.Application;
using UserService.Application.Persistence;
using UserService.Application.Services;
using ChargeSucceededBackgroundService = UserService.Application.Services.ChargeSucceededBackgroundService;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddOptions<PostgresConfiguration>()
    .Bind(builder.Configuration.GetSection("Database"))
    .ValidateDataAnnotations();

builder.Services.AddOptions<KafkaConfiguration>()
    .Bind(builder.Configuration.GetSection("Database"))
    .ValidateDataAnnotations();

string? kafkaServers = builder.Configuration.GetSection("Kafka")["Servers"];
if (string.IsNullOrWhiteSpace(kafkaServers))
{
    throw new InvalidDataException("Kafka__Servers not defined");
}

// Cant currently use normal AddOptions as KafkaConfiguration does not have a default constructor
builder.Services.AddSingleton(Options.Create(new KafkaConfiguration(
    kafkaServers
)));

builder.Services.AddDbContext<DatabaseContext>();
builder.Services.AddApplicationAuth(builder);
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(MediatREntryPoint).Assembly));
builder.Services.AddScoped<ICheckoutSucceededEventListener, ChargeSucceededBackgroundService>();
builder.Services.AddHostedService<UserService.Presentation.Background.ChargeSucceededBackgroundService>();

builder.Services.AddControllers();

WebApplication app = builder.Build();

app.UseRouting();

app.UseApplicationAuth();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();