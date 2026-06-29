using PaymentService.Domain.src.Enums;

namespace PaymentService.Application.src.Interfaces;

public interface IPaymentSessionFactory
{
    IProviderPaymentSessionService CreatePaymentSessionService(PaymentProvider provider);
}