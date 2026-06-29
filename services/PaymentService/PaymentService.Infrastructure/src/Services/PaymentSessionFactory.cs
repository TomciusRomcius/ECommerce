using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PaymentService.Application.src.Interfaces;
using PaymentService.Domain.src.Enums;
using PaymentService.Infrastructure.src.Utils;

namespace PaymentService.Infrastructure.src.Services;

public class PaymentSessionFactory : IPaymentSessionFactory
{
    private readonly ILogger<PaymentSessionFactory> _logger;
    private readonly StripeSettings _stripeSettings;

    public PaymentSessionFactory(ILogger<PaymentSessionFactory> logger, IOptions<StripeSettings> stripeSettings)
    {
        _logger = logger;
        _stripeSettings = stripeSettings.Value;
    }

    public IProviderPaymentSessionService CreatePaymentSessionService(PaymentProvider provider)
    {
        _logger.LogTrace("Entered CreatePaymentSessionService");
        _logger.LogDebug("Creating payment session service with provider enum value being: {}", provider);
        IProviderPaymentSessionService? result = null;

        if (provider == PaymentProvider.STRIPE) result = new StripeSessionService(_stripeSettings);

        if (result is null) throw new InvalidOperationException("Invalid payment provider");

        return result;
    }
}