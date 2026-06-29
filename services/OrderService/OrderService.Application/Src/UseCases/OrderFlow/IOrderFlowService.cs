using OrderService.Application.UseCases.Payment;
using OrderService.Application.Utils;

namespace OrderService.Application.UseCases.OrderFlow;

public interface IOrderFlowService
{
    Task<Result<PaymentSessionModel>> CreateOrderPaymentSession(Guid userId, PaymentProvider paymentProvider);
}
