using Microsoft.AspNetCore.Mvc;

namespace BFF.Utils;

public static class ControllerUtils
{
    public static IActionResult ResultErrorToResponse(ResultError error)
    {
        if (error.ErrorType == ResultErrorType.UPSTREAM_ERROR)
        {
            return HttpResponseUtils.FromStringBody(error.HttpStatusCode ?? StatusCodes.Status502BadGateway, error.Message);
        }

        int statusCode = ResultErrorTypeToStatusCode(error.ErrorType);

        return new ObjectResult(error)
        {
            StatusCode = statusCode,
        };
    }

    public static IActionResult ResultErrorsToResponse(IEnumerable<ResultError> errors) =>
        ResultErrorToResponse(errors.First());

    private static int ResultErrorTypeToStatusCode(ResultErrorType errorType) =>
        errorType switch
        {
            ResultErrorType.UNKNOWN_ERROR => StatusCodes.Status500InternalServerError,
            ResultErrorType.VALIDATION_ERROR => StatusCodes.Status403Forbidden,
            ResultErrorType.INVALID_OPERATION_ERROR => StatusCodes.Status403Forbidden,
            ResultErrorType.UNAUTHORIZED => StatusCodes.Status401Unauthorized,
            ResultErrorType.NOT_FOUND => StatusCodes.Status404NotFound,
            ResultErrorType.GATEWAY_ERROR => StatusCodes.Status502BadGateway,
            _ => StatusCodes.Status500InternalServerError,
        };
}
