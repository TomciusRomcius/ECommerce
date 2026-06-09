using System.Reflection;
using ECommerceBackend.Utils.Auth;
using ECommerceBackend.Utils.Database;
using OrderService.Application.Persistence;
using OrderService.Presentation.ServiceUseCases.Payment;
using OrderService.Presentation.ServiceUseCases.UserCart;
using OrderService.Presentation.ServiceUseCases.Utils;
using OrderService.Presentation.UseCases.OrderFlow;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging();
builder.Services.AddHttpClient();
builder.Services.AddMediatR(options => options.RegisterServicesFromAssembly(
        Assembly.GetExecutingAssembly()
    )
);

builder.Services.AddScoped<IOrderFlowService, OrderFlowService>();
builder.Services.AddScoped<IPaymentSessionService, PaymentSessionService>();
builder.Services.AddScoped<IOrderPriceCalculator, OrderPriceCalculator>();
builder.Services.AddScoped<IUserCartService, UserCartService>();
builder.Services.AddOptions<PostgresConfiguration>()
    .Bind(builder.Configuration.GetSection("Database"))
    .ValidateDataAnnotations()
    .ValidateOnStart();
builder.Services.AddDbContext<DatabaseContext>();

builder.Services.AddOptions<MicroserviceNetworkConfig>()
    .Bind(builder.Configuration.GetSection("MicroserviceNetworkConfig"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddApplicationAuth(builder);
builder.Services.AddBackgroundJwtRefresher();
builder.Services.AddOpenApi();
builder.Services.AddControllers();

builder.Services.AddScoped<OrderService.Application.Services.IOrderService, OrderService.Application.Services.OrderService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseApplicationAuth();
app.MapControllers();
app.Run();
