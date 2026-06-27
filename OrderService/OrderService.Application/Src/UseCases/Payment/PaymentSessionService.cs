using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OrderService.Application.Utils;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace OrderService.Application.UseCases.Payment;

public class PaymentSessionService : IPaymentSessionService
{
    private readonly HttpClient _httpClient;
    private readonly MicroserviceNetworkConfig _networkConfig;
    private readonly ILogger<PaymentSessionService> _logger;

    public PaymentSessionService(
        HttpClient httpClient,
        IOptions<MicroserviceNetworkConfig> networkConfig,
        ILogger<PaymentSessionService> logger)
    {
        _httpClient = httpClient;
        _networkConfig = networkConfig.Value;
        _logger = logger;
    }

    public async Task<Result<PaymentSessionModel>> GeneratePaymentSessionAsync(
        Guid orderId,
        GeneratePaymentSessionOptions sessionOptions)
    {
        _logger.LogTrace("Entered PaymentSessionService.GeneratePaymentSessionAsync");
        _logger.LogDebug("Creating session with options: {@PaymentSessionOptions}", sessionOptions);

        var jsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };
        string messageSerialized = JsonConvert.SerializeObject(sessionOptions, jsonSerializerSettings);
        Dictionary<string, object>? messageDictionary =
            JsonConvert.DeserializeObject<Dictionary<string, object>>(messageSerialized);
        if (messageDictionary == null)
        {
            return new Result<PaymentSessionModel>([
                new ResultError(
                    ResultErrorType.UNKNOWN_ERROR,
                    "Failed to format the http request for generating a payment session!")
            ]);
        }

        messageDictionary["orderId"] = orderId;
        string requestString = JsonConvert.SerializeObject(messageDictionary);
        StringContent httpContent = new(requestString, Encoding.UTF8, "application/json");

        HttpResponseMessage? response =
            await _httpClient.PostAsync($"{_networkConfig.PaymentServiceUrl}/paymentsession", httpContent);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError(
                "Failed to create payment session. HTTP body: {HttpBody}. HTTP Status code: {StatusCode}",
                await response.Content.ReadAsStringAsync(),
                response.StatusCode);
            return new Result<PaymentSessionModel>([
                new ResultError(ResultErrorType.UNKNOWN_ERROR, "Failed to create payment session")
            ]);
        }

        string json = await response.Content.ReadAsStringAsync();
        _logger.LogDebug("Payment session json: {}", json);
        PaymentSessionModel? session = JsonUtils.Deserialize<PaymentSessionModel>(json);
        if (session == null)
        {
            _logger.LogError(
                "Failed to create payment session. Payment service response was either empty or current response model is malformed");
            return new Result<PaymentSessionModel>([
                new ResultError(ResultErrorType.UNKNOWN_ERROR, "Failed to create payment session")
            ]);
        }

        _logger.LogTrace(
            "Created payment session for order {OrderId}, user {UserId}",
            orderId,
            sessionOptions.UserId);

        return new Result<PaymentSessionModel>(session);
    }

    public async Task<Result<PaymentSessionModel?>> GetPaymentSessionAsync(Guid userId)
    {
        _logger.LogTrace("Entered PaymentSessionService.GetPaymentSessionAsync");
        HttpResponseMessage? response =
            await _httpClient.GetAsync($"{_networkConfig.PaymentServiceUrl}/paymentsession?userId={userId}");
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError(
                "Failed to get user payment session. UserId: {UserId}. Http body: {HttpBody}. Http status code: {StatusCode}.",
                userId,
                await response.Content.ReadAsStringAsync(),
                response.StatusCode);
            return new Result<PaymentSessionModel?>([
                new ResultError(ResultErrorType.UNKNOWN_ERROR, "Failed to create payment session")
            ]);
        }

        string json = await response.Content.ReadAsStringAsync();
        PaymentSessionModel? session = JsonSerializer.Deserialize<PaymentSessionModel>(json);
        return new Result<PaymentSessionModel?>(session);
    }
}
