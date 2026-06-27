using PaymentService.Domain.src.Entities;
using PaymentService.Domain.src.Enums;
using PaymentService.Domain.src.Utils;

namespace PaymentService.Application.src.Interfaces
{
    public interface IPaymentSessionCoordinator
    {
        Task<Result<PaymentSessionEntity?>> CreatePaymentSessionAsync(PaymentProvider paymentProvider, GeneratePaymentSessionOptions options);
        Task<PaymentSessionEntity?> GetUserSessionAsync(Guid userId);
        /// <summary>
        /// Verifies whether the user has paid and if yes, also publishes a CheckoutSucceededEvent
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="checkoutId"></param>
        /// <returns></returns>
        Task<bool> VerifyPaymentAsync(PaymentProvider provider, string checkoutId);
    }
}