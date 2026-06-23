namespace BFF.OrderFlow;

public interface IOrderFlowService
{
    Task<HttpResponseMessage> CreateOrderPaymentSessionAsync(
        string? authorizationHeader,
        CancellationToken cancellationToken = default);
}
