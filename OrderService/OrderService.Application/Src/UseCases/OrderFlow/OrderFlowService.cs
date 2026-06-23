using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrderService.Application.Persistence;
using OrderService.Application.UseCases.Payment;
using OrderService.Application.UseCases.UserCart;
using OrderService.Application.Utils;
using OrderService.Domain.Entities;

namespace OrderService.Application.UseCases.OrderFlow;

public class OrderFlowService : IOrderFlowService
{
    private readonly DatabaseContext _dbContext;
    private readonly ILogger<OrderFlowService> _logger;
    private readonly IOrderPriceCalculator _orderPriceCalculator;
    private readonly IPaymentSessionService _paymentSessionService;
    private readonly IUserCartService _userCartService;

    public OrderFlowService(
        ILogger<OrderFlowService> logger,
        IPaymentSessionService paymentSessionService,
        IOrderPriceCalculator orderPriceCalculator,
        IUserCartService userCartService,
        DatabaseContext dbContext)
    {
        _logger = logger;
        _paymentSessionService = paymentSessionService;
        _orderPriceCalculator = orderPriceCalculator;
        _userCartService = userCartService;
        _dbContext = dbContext;
    }

    public async Task<Result<PaymentSessionModel>> CreateOrderPaymentSession(
        Guid userId,
        PaymentProvider paymentProvider)
    {
        var hasActiveOrder = await _dbContext.Orders.AsNoTracking()
            .Where(o => o.OrderState == OrderState.Active)
            .AnyAsync();

        if (hasActiveOrder)
        {
            await _dbContext.Orders.Where(o => o.OrderState == OrderState.Active)
                .ExecuteDeleteAsync();

            // TODO: publish Kafka event and make the payment service delete the stripe payment session
        }

        Guid orderId = Guid.NewGuid();

        _logger.LogTrace("Entered CreateOrderPaymentSession");
        _logger.LogDebug("Creating order payment session for user: {UserId}", userId);

        Result<IEnumerable<CartProductModel>> cartProductsResult =
            await _userCartService.GetUserCartProductModelsAsync(userId);
        if (cartProductsResult.Errors.Any())
        {
            return new Result<PaymentSessionModel>(cartProductsResult.Errors);
        }

        IEnumerable<CartProductModel> cartProducts = cartProductsResult.GetValue();

        if (!cartProducts.Any())
        {
            return new Result<PaymentSessionModel>([
                new ResultError(ResultErrorType.INVALID_OPERATION_ERROR, "Cannot create an order with zero items in the cart.")
            ]);
        }

        decimal price = cartProducts.Sum(cp => cp.Price * cp.Quantity);

        if (price <= 0)
        {
            throw new InvalidOperationException("Price is less or equal to zero!");
        }

        Result<PaymentSessionModel> intentSession = await _paymentSessionService.GeneratePaymentSessionAsync(
            orderId,
            new GeneratePaymentSessionOptions
            {
                PaymentProvider = paymentProvider,
                PriceCents = Convert.ToInt32(price * 100),
                UserId = userId.ToString()
            });

        if (intentSession.Errors.Any())
        {
            _logger.LogError("Failed to create payment intent for user: {UserId}", userId);
            return new Result<PaymentSessionModel>([intentSession.Errors.First()]);
        }

        _logger.LogTrace("Created payment session for user: {UserId}", userId);

        IEnumerable<OrderProductEntity> orderProducts = cartProducts.Select(cp => new OrderProductEntity
        {
            OrderId = orderId,
            ProductId = cp.ProductId,
            StoreLocationId = cp.StoreLocationId,
            ProductName = "TODO",
            Quantity = cp.Quantity
        });

        OrderEntity order = new() { OrderEntityId = orderId, UserId = userId, OrderState = OrderState.Active };

        _dbContext.OrderProducts.AddRange(orderProducts);
        _dbContext.Orders.Add(order);
        await _dbContext.SaveChangesAsync();

        return new Result<PaymentSessionModel>(intentSession.GetValue());
    }
}
