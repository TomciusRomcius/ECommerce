using OrderService.Application.Services;
using OrderService.Application.UseCases.Payment;
using OrderService.Application.UseCases.ProductReservation;
using OrderService.Application.UseCases.UserCart;
using OrderService.Application.Utils;
using OrderService.Domain.Entities;

namespace OrderService.Application.UseCases.OrderFlow;

public class OrderFlowService : IOrderFlowService
{
    private readonly IOrderPriceCalculator _orderPriceCalculator;
    private readonly IOrderService _orderService;
    private readonly IPaymentSessionService _paymentSessionService;
    private readonly IProductReservationService _productReservationService;
    private readonly IUserCartService _userCartService;

    public OrderFlowService(
        IPaymentSessionService paymentSessionService,
        IOrderPriceCalculator orderPriceCalculator,
        IProductReservationService productReservationService,
        IUserCartService userCartService,
        IOrderService orderService)
    {
        _paymentSessionService = paymentSessionService;
        _orderPriceCalculator = orderPriceCalculator;
        _productReservationService = productReservationService;
        _userCartService = userCartService;
        _orderService = orderService;
    }

    public async Task<Result<PaymentSessionModel>> CreateOrderPaymentSession(
        Guid userId,
        PaymentProvider paymentProvider)
    {
        await _orderService.DeleteActiveOrdersAsync(userId);
        await _paymentSessionService.DeletePaymentSessionAsync(userId);

        Guid orderId = Guid.NewGuid();

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

        ResultError? reservationError = await _productReservationService.ReserveProductsAsync(orderId, cartProducts);
        if (reservationError is not null)
        {
            return new Result<PaymentSessionModel>([reservationError]);
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
            return new Result<PaymentSessionModel>([intentSession.Errors.First()]);
        }

        IEnumerable<OrderProductEntity> orderProducts = cartProducts.Select(cp => new OrderProductEntity
        {
            OrderId = orderId,
            ProductId = cp.ProductId,
            StoreLocationId = cp.StoreLocationId,
            ProductName = "TODO",
            Quantity = cp.Quantity
        });

        OrderEntity order = new() { OrderEntityId = orderId, UserId = userId, OrderState = OrderState.Active };

        await _orderService.CreateOrderWithProductsAsync(order, orderProducts);

        return new Result<PaymentSessionModel>(intentSession.GetValue());
    }
}
