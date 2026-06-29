namespace PaymentService.Application.src.Interfaces;

public interface IWebhookCoordinatorService
{
    /// <summary>
    /// Parses webhook event and for specific events publishes kafka events and executes logic using a strategy map.
    /// </summary>
    Task HandlePaymentWebhook(string json, string signature);
}