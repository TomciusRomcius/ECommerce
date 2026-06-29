using Microsoft.AspNetCore.Mvc;
using ProductService.Domain.Utils;

namespace ProductService.Presentation.Utils;

public static class ControllerUtils
{
    public static IActionResult ResultErrorToResponse(ResultError error)
    {
        var statusCode = ResultErrorTypeToStatusCode(error.ErrorType);

        var problemDetails = new ProblemDetails
        {
            Title = error.Message,
            Detail = error.Message,
            Status = statusCode
        };

        return new ObjectResult(error)
        {
            StatusCode = statusCode
        };
    }

    public static IActionResult ResultErrorsToResponse(IEnumerable<ResultError> errors)
    {
        return ResultErrorToResponse(errors.First());
    }

    private static int ResultErrorTypeToStatusCode(ResultErrorType errorType)
    {
        return errorType switch
        {
            ResultErrorType.UNKNOWN_ERROR => 500,
            ResultErrorType.VALIDATION_ERROR => 403,
            ResultErrorType.INVALID_OPERATION_ERROR => 403,
            ResultErrorType.UNAUTHORIZED => 401,
            _ => 500
        };
    }
}