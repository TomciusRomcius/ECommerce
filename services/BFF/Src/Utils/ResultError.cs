namespace BFF.Utils;

public enum ResultErrorType
{
    UNKNOWN_ERROR = 0,
    VALIDATION_ERROR,
    INVALID_OPERATION_ERROR,
    UNAUTHORIZED,
    UNSUPPORTED,
    NOT_FOUND,
    UPSTREAM_ERROR,
    GATEWAY_ERROR,
}

public class ResultError
{
    public ResultError(ResultErrorType errorType, string message, int? httpStatusCode = null)
    {
        ErrorType = errorType;
        Message = message;
        HttpStatusCode = httpStatusCode;
    }

    public ResultErrorType ErrorType { get; init; }
    public string Message { get; init; }
    public int? HttpStatusCode { get; init; }
}
