using Microsoft.AspNetCore.Mvc;
using OrderService.Application.Utils;

namespace OrderService.Presentation.Utils;

public static class ControllerUtils
{
    public static IActionResult ResultErrorToResponse(ResultError error)
    {
        int statusCode = ResultErrorTypeToStatusCode(error.ErrorType);

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
            _ => 500,
        };
    }
}
