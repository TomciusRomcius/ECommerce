using OrderService.Payment;
using OrderService.Utils;

namespace OrderService.Presentation.UseCases.OrderFlow;

public interface IOrderFlowService
{
    public Task<Result<PaymentSessionModel>> CreateOrderPaymentSession(Guid userId, PaymentProvider paymentProvider);
}
