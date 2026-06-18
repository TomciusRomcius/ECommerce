using Amazon.S3;
using BFF.Auth;
using BFF.Cart;
using BFF.Configuration;
using BFF.Order;
using BFF.Products;
using BFF.ReadDb;
using BFF.StoreProducts;
using ECommerceBackend.Utils.Auth;
using ECommerceBackend.Utils.Database;
using ECommerceBackend.Utils.Microservices;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddLogging();
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
builder.Services.AddHttpClient();
builder.Services.AddAutoMapper(
    _ => { },
    typeof(StoreProductsMappingProfile));
builder.Services.AddOptions<KeycloakAuthOptions>()
    .Bind(builder.Configuration.GetSection(KeycloakAuthOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();
builder.Services.AddHttpClient<IKeycloakTokenService, KeycloakTokenService>();
builder.Services.AddHttpClient<ICartService, CartService>();
builder.Services.AddHttpClient<IOrderPaymentSessionService, OrderPaymentSessionService>();
builder.Services.AddScoped<IStoreProductsService, StoreProductsService>();
builder.Services.AddHttpClient<IProductService, ProductService>();
builder.Services.AddOptions<PostgresConfiguration>()
    .Bind(builder.Configuration.GetSection("Database"))
    .ValidateDataAnnotations()
    .ValidateOnStart();
builder.Services.AddDbContext<ReadDbContext>();
builder.Services.AddOptions<MicroserviceHosts>()
    .Bind(builder.Configuration.GetSection("MicroserviceNetworkConfig"))
    .ValidateDataAnnotations()
    .ValidateOnStart();
builder.Services.AddOptions<S3Configuration>()
    .Bind(builder.Configuration.GetSection(S3Configuration.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();
builder.Services.AddScoped<StoreProductImageUrlsMappingAction>();
builder.Services.AddSingleton<IS3ImageUrlBuilder, S3ImageUrlBuilder>();
builder.Services.AddApplicationAuth(builder);
builder.Services.AddSingleton<IAmazonS3>(sp =>
{
    S3Configuration s3 = sp.GetRequiredService<IOptions<S3Configuration>>().Value;
    var s3Config = new AmazonS3Config
    {
        ServiceURL = s3.ServiceUrl,
        ForcePathStyle = true,
        AuthenticationRegion = s3.Region,
    };
    return new AmazonS3Client(s3.AwsAccessKeyId, s3.AwsSecretAccessKey, s3Config);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors();
app.MapControllers();

app.Run();
