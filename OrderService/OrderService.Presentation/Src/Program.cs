using ECommerceBackend.Utils.Auth;
using ECommerceBackend.Utils.Database;
using OrderService.Application;
using OrderService.Application.Persistence;
using OrderService.Application.Services;
using OrderService.Application.UseCases.OrderFlow;
using OrderService.Application.UseCases.Payment;
using OrderService.Application.UseCases.ProductReservation;
using OrderService.Application.UseCases.UserCart;
using OrderService.Application.Utils;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging();
builder.Services.AddHttpClient();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(MediatREntryPoint).Assembly));

builder.Services.AddScoped<IOrderFlowService, OrderFlowService>();
builder.Services.AddScoped<IPaymentSessionService, PaymentSessionService>();
builder.Services.AddScoped<IProductReservationService, ProductReservationService>();
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

builder.Services.AddScoped<IOrderService, OrderService.Application.Services.OrderService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseApplicationAuth();
app.MapControllers();
app.Run();
