namespace StoreService.Domain.Utils;

public enum ResultErrorType
{
    UNKNOWN_ERROR = 0,
    VALIDATION_ERROR,
    INVALID_OPERATION_ERROR,
    UNAUTHORIZED,
    UNSUPPORTED
}

public class ResultError
{
    public ResultError(ResultErrorType errorType, string message)
    {
        ErrorType = errorType;
        Message = message;
    }

    public ResultErrorType ErrorType { get; init; }
    public string Message { get; init; }
}