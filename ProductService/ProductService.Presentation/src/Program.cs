using ProductService.Application;
using ProductService.Application.Mapping;
using ProductService.Application.Persistence;
using ProductService.Application.Services;
using ECommerceBackend.Utils.Auth;
using ECommerceBackend.Utils.Database;
using ProductService.Presentation.Mapping;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddAutoMapper(
    _ => { },
    typeof(ApplicationMappingProfile),
    typeof(PresentationMappingProfile));

builder.Services.AddOptions<PostgresConfiguration>()
    .Bind(builder.Configuration.GetSection("Database"))
    .ValidateDataAnnotations();

builder.Services.AddDbContext<DatabaseContext>();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(MediatREntryPoint).Assembly));
builder.Services.AddScoped<ICategoriesService, CategoriesService>();
builder.Services.AddApplicationAuth(builder);

var app = builder.Build();

if (app.Environment.IsDevelopment()) app.MapOpenApi();

app.MapControllers();
app.UseHttpsRedirection();
app.UseApplicationAuth();

app.Run();