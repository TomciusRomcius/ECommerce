using OrderService.Presentation.ServiceUseCases.Payment;
using OrderService.Presentation.ServiceUseCases.Utils;

namespace OrderService.Presentation.UseCases.OrderFlow;

public interface IOrderFlowService
{
    public Task<Result<PaymentSessionModel>> CreateOrderPaymentSession(Guid userId, PaymentProvider paymentProvider);
}
