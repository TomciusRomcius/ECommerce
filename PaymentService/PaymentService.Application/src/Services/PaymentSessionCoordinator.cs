using ECommerceBackend.EventTypes;
using EventSystemHelper.Kafka.Services;
using EventSystemHelper.Kafka.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PaymentService.Application.src.Interfaces;
using PaymentService.Application.src.Models;
using PaymentService.Domain.src.Entities;
using PaymentService.Domain.src.Enums;
using PaymentService.Domain.src.Models;
using PaymentService.Domain.src.Utils;

namespace PaymentService.Application.src.Services
{
    public class PaymentSessionCoordinator : IPaymentSessionCoordinator
    {
        private readonly ILogger<PaymentSessionCoordinator> _logger;
        private readonly IPaymentSessionFactory _paymentSessionFactory;
        private readonly IPaymentSessionPersistenceService _paymentSessionPersistenceService;
        private readonly KafkaConfiguration _kafkaConfiguration;

        public PaymentSessionCoordinator(
            ILogger<PaymentSessionCoordinator> logger,
            IPaymentSessionFactory paymentSessionFactory,
            IPaymentSessionPersistenceService paymentSessionPersistenceService,
            IOptions<KafkaConfiguration> kafkaConfiguration)
        {
            _logger = logger;
            _paymentSessionFactory = paymentSessionFactory;
            _paymentSessionPersistenceService = paymentSessionPersistenceService;
            _kafkaConfiguration = kafkaConfiguration.Value;
        }

        public async Task<Result<PaymentSessionEntity?>> CreatePaymentSessionAsync(
            PaymentProvider paymentProvider,
            GeneratePaymentSessionOptions options)
        {
            _logger.LogTrace("CreatePaymentSessionAsync");
            _logger.LogDebug(
                "Creating payment session with provider: {Provider}, options: {@Options}",
                paymentProvider,
                options
            );

            if (options.Price <= 0)
            {
                return new Result<PaymentSessionEntity?>([
                    new ResultError(ResultErrorType.VALIDATION_ERROR, "Price must be greater than 0")
                ]);
            }

            IProviderPaymentSessionService paymentSessionProviderService = _paymentSessionFactory.CreatePaymentSessionService(
                paymentProvider
            );

            PaymentProviderSession session = await paymentSessionProviderService.GeneratePaymentSession(options);

            var entity = new PaymentSessionEntity
            {
                UserId = options.UserId,
                PaymentSessionId = session.SessionId,
                CheckoutUrl = session.CheckoutUrl,
                PaymentSessionProvider = paymentProvider
            };

            ResultError? persistanceErr = await _paymentSessionPersistenceService.CreateAsync(entity);

            if (persistanceErr != null)
            {
                // TODO: delete payment session
                return new Result<PaymentSessionEntity?>([persistanceErr]);
            }

            return new Result<PaymentSessionEntity?>(entity);
        }

        public async Task<PaymentSessionEntity?> GetUserSessionAsync(Guid userId)
        {
            _logger.LogTrace("Entered GetUserSessionAsync");
            _logger.LogDebug("Getting payment session for user id: {UserId}", userId.ToString());
            return await _paymentSessionPersistenceService.GetUserSessionAsync(userId);
        }

        public async Task<ResultError?> DeleteUserSessionAsync(Guid userId)
        {
            _logger.LogTrace("Entered DeleteUserSessionAsync");
            _logger.LogDebug("Deleting payment session for user {UserId}", userId);
            return await _paymentSessionPersistenceService.DeleteAsync(userId);
        }

        public async Task<bool> VerifyPaymentAsync(PaymentProvider provider, string checkoutId)
        {
            IProviderPaymentSessionService? providerService = _paymentSessionFactory.CreatePaymentSessionService(provider);
            PaymentSessionDetails? details = await providerService.GetPaymentSessionDetails(checkoutId);
            if (details is null)
            {
                // TODO: result pattern
                return false;
            }

            if (!details.IsPaid)
            {
                return false;
            }

            var producer = new KafkaEventProducer(_kafkaConfiguration);
            var sMessage = new CheckoutSucceededEvent
            {
                UserId = details.UserId,
                OrderId = details.OrderId,
                Amount = (int)details.Amount // TODO: long
            };

            return true;
        }
    }
}
