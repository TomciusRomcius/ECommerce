namespace PaymentService.Infrastructure.src.Utils;

public class QueryParameter
{
    public QueryParameter(object? value)
    {
        Value = value;
    }

    public QueryParameter(string key, object? value)
    {
        Key = key;
        Value = value;
    }

    public string? Key { get; set; }
    public object? Value { get; set; }
}