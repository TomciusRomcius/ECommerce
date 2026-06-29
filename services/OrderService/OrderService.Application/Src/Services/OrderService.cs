using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrderService.Application.Persistence;
using OrderService.Application.Utils;
using OrderService.Domain.Entities;

namespace OrderService.Application.Services;

public interface IOrderService
{
    Task<ResultError?> CreateOrderAsync(OrderEntity order);
    Task CreateOrderWithProductsAsync(OrderEntity order, IEnumerable<OrderProductEntity> orderProducts);
    Task DeleteActiveOrdersAsync(Guid userId);
    Task<OrderEntity?> GetOrderAsync(Guid userId, Guid orderId);
}

public class OrderService : IOrderService
{
    private readonly ILogger<OrderService> _logger;
    private readonly DatabaseContext _dbContext;
    
    public OrderService(ILogger<OrderService> logger, DatabaseContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<ResultError?> CreateOrderAsync(OrderEntity order)
    {
        // TODO: validation
        _dbContext.Orders.Add(order);
        await _dbContext.SaveChangesAsync();
        return null;
    }

    public async Task CreateOrderWithProductsAsync(
        OrderEntity order,
        IEnumerable<OrderProductEntity> orderProducts)
    {
        _logger.LogTrace("Entered {MethodName}", nameof(CreateOrderWithProductsAsync));
        _logger.LogDebug(
            "Creating order {OrderId} for user {UserId} with {ProductCount} products",
            order.OrderEntityId,
            order.UserId,
            orderProducts.Count());

        _dbContext.OrderProducts.AddRange(orderProducts);
        _dbContext.Orders.Add(order);
        await _dbContext.SaveChangesAsync();

        _logger.LogDebug(
            "Successfully created order {OrderId} for user {UserId}",
            order.OrderEntityId,
            order.UserId);
    }

    public async Task DeleteActiveOrdersAsync(Guid userId)
    {
        _logger.LogTrace("Entered {MethodName}", nameof(DeleteActiveOrdersAsync));

        bool hasActiveOrder = await _dbContext.Orders.AsNoTracking()
            .Where(o => o.OrderState == OrderState.Active && o.UserId == userId)
            .AnyAsync();

        if (!hasActiveOrder)
        {
            return;
        }

        int deletedCount = await _dbContext.Orders
            .Where(o => o.OrderState == OrderState.Active && o.UserId == userId)
            .ExecuteDeleteAsync();

        _logger.LogDebug(
            "Deleted {DeletedCount} stale active order(s) for user {UserId}",
            deletedCount,
            userId);
    }

    public async Task<OrderEntity?> GetOrderAsync(Guid userId, Guid orderId)
    {
        OrderEntity? order = await _dbContext.Orders
            .Where(o => o.UserId == userId && o.OrderEntityId == orderId)
            .FirstOrDefaultAsync();

        return order;
    }
}
