using ECommerceBackend.Utils.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using StoreService.Application.Persistence;
using StoreService.Application.UseCases.ProductStoreLocations.Commands;
using StoreService.Application.UseCases.ProductStoreLocations.Handlers;
using StoreService.Domain.Entities;
using StoreService.Domain.Utils;
using Testcontainers.PostgreSql;

namespace StoreService.Application.Tests.Integration;

public class ReserveProductsHandlerTest
{
    [Fact]
    public async Task Handle_ShouldReserveProductAndReduceStock_WhenStockIsSufficient()
    {
        DatabaseContext dbContext = await SetupDatabaseContext();
        ReserveProductsHandler handler = new(
            new Mock<ILogger<ReserveProductsHandler>>().Object,
            dbContext);

        StoreLocationEntity store = new("Store 1", "Address 1");
        dbContext.StoreLocations.Add(store);
        await dbContext.SaveChangesAsync();

        ProductStoreLocationEntity productStore = new(store.StoreLocationId, productId: 1, stock: 10);
        dbContext.ProductStoreLocations.Add(productStore);
        await dbContext.SaveChangesAsync();

        Guid orderId = Guid.NewGuid();
        ReserveProductsCommand command = new(orderId, [new ReserveProductItem(store.StoreLocationId, 1, 3)]);

        ResultError? error = await handler.Handle(command, CancellationToken.None);

        Assert.Null(error);

        ProductStoreLocationEntity updatedProductStore = await dbContext.ProductStoreLocations
            .SingleAsync(psl => psl.StoreLocationId == store.StoreLocationId && psl.ProductId == 1);
        Assert.Equal(7, updatedProductStore.Stock);

        ReservedProductEntity reservedProduct = await dbContext.ReservedProducts.SingleAsync();
        Assert.Equal(orderId, reservedProduct.OrderId);
        Assert.Equal(3, reservedProduct.Stock);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenStockIsInsufficient()
    {
        DatabaseContext dbContext = await SetupDatabaseContext();
        ReserveProductsHandler handler = new(
            new Mock<ILogger<ReserveProductsHandler>>().Object,
            dbContext);

        StoreLocationEntity store = new("Store 1", "Address 1");
        dbContext.StoreLocations.Add(store);
        await dbContext.SaveChangesAsync();

        ProductStoreLocationEntity productStore = new(store.StoreLocationId, productId: 1, stock: 2);
        dbContext.ProductStoreLocations.Add(productStore);
        await dbContext.SaveChangesAsync();

        ReserveProductsCommand command = new(
            Guid.NewGuid(),
            [new ReserveProductItem(store.StoreLocationId, 1, 5)]);

        ResultError? error = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(error);
        Assert.Equal(ResultErrorType.INVALID_OPERATION_ERROR, error.ErrorType);

        ProductStoreLocationEntity unchangedProductStore = await dbContext.ProductStoreLocations
            .SingleAsync(psl => psl.StoreLocationId == store.StoreLocationId && psl.ProductId == 1);
        Assert.Equal(2, unchangedProductStore.Stock);
        Assert.Empty(await dbContext.ReservedProducts.ToListAsync());
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenProductNotFoundInStore()
    {
        DatabaseContext dbContext = await SetupDatabaseContext();
        ReserveProductsHandler handler = new(
            new Mock<ILogger<ReserveProductsHandler>>().Object,
            dbContext);

        StoreLocationEntity store = new("Store 1", "Address 1");
        dbContext.StoreLocations.Add(store);
        await dbContext.SaveChangesAsync();

        ReserveProductsCommand command = new(
            Guid.NewGuid(),
            [new ReserveProductItem(store.StoreLocationId, 99, 1)]);

        ResultError? error = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(error);
        Assert.Equal(ResultErrorType.INVALID_OPERATION_ERROR, error.ErrorType);
        Assert.Empty(await dbContext.ReservedProducts.ToListAsync());
    }

    private static async Task<DatabaseContext> SetupDatabaseContext()
    {
        PostgreSqlContainer postgresql = new PostgreSqlBuilder("postgres:17")
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
