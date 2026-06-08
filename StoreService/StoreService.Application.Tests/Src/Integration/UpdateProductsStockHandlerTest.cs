using ECommerceBackend.Utils.Database;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using StoreService.Application.Persistence;
using StoreService.Application.UseCases.AvailableProducts.Handlers;
using StoreService.Application.UseCases.ProductStoreLocations.Commands;
using StoreService.Application.UseCases.ProductStoreLocations.Handlers;
using StoreService.Domain.Entities;
using Testcontainers.PostgreSql;

namespace StoreService.Application.Tests.Integration;

public class UpdateProductsStockHandlerTest
{
    private const string PostgresImage = "postgres:17";
    
    [Fact] 
    public async Task Handle_ShouldCorrectlyUpdateProductStock_WhenListContainsOneProduct()
    {
        // ARRANGE:
        DatabaseContext dbContext = await SetupDatabaseContext();
        
        UpdateProductsStockHandler handler = new(
            new Mock<ILogger<UpdateProductsStockHandler>>().Object,
            dbContext);

        List<StoreLocationEntity> stores =
        [
            new("Store 1", "Address 1"), new("Store 2", "Address 2")
        ];

        dbContext.StoreLocations.AddRange(stores);
        await dbContext.SaveChangesAsync();

        List<ProductStoreLocationEntity> productStores =
        [
            new(stores[0].StoreLocationId, 0, 5),
            new(stores[1].StoreLocationId, 2, 3),
            new(stores[0].StoreLocationId, 5, 14),
        ];

        dbContext.ProductStoreLocations.AddRange(productStores);
        await dbContext.SaveChangesAsync();

        List<ProductStoreLocationEntity> updator =
        [
            new(stores[0].StoreLocationId, 0, 1),
            new(stores[1].StoreLocationId, 1, 2),
        ];
        
        UpdateProductsStockCommand cmd = new(updator);
        await handler.Handle(cmd, CancellationToken.None);
        
        // ACT:
        ExpressionStarter<ProductStoreLocationEntity>? predicate = PredicateBuilder.New<ProductStoreLocationEntity>(false);
        
        foreach (ProductStoreLocationEntity ps in productStores)
        {
            predicate = predicate.Or(x => x.StoreLocationId == ps.StoreLocationId
                              &&  x.ProductId == ps.ProductId);
        }
        
        Dictionary<(int, int), ProductStoreLocationEntity> results = await dbContext.ProductStoreLocations
            .AsExpandable()
            .Where(predicate)
            .ToDictionaryAsync(x => (x.StoreLocationId, x.ProductId));
        
        // ASSERT:
        foreach (ProductStoreLocationEntity productStoreLocation in productStores)
        {
            Assert.Equal(productStoreLocation.Stock, results[(productStoreLocation.StoreLocationId, productStoreLocation.ProductId)].Stock);
        }
    }
    
    private async Task<DatabaseContext> SetupDatabaseContext()
    {
        PostgreSqlContainer? postgresql = new PostgreSqlBuilder("postgres:17")
            .WithDatabase("postgres")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

        await postgresql.StartAsync();

        PostgresConfiguration postgresConfig = new()
        {
            Database = "postgres",
            Host = postgresql.IpAddress,
            Username = "postgres",
            Password = "postgres",
        };
        
        DatabaseContext dbContext = new(Options.Create(postgresConfig));
        await dbContext.Database.MigrateAsync();
        return dbContext;
    }
}
