using PaymentService.Domain.src.Entities;
using PaymentService.Domain.src.Utils;

namespace PaymentService.Application.src.Interfaces
{
    public interface IPaymentSessionPersistenceService
    {
        Task<ResultError?> CreateAsync(PaymentSessionEntity entity);
        Task<PaymentSessionEntity?> GetUserSessionAsync(Guid userId);
        Task<ResultError?> DeleteAsync(Guid userId);
    }
}
