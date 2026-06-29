using Microsoft.AspNetCore.Mvc;
using PaymentService.Domain.src.Utils;

namespace ECommerce.Presentation.src.Utils
{
    public static class ControllerUtils
    {
        public static IActionResult ResultErrorToResponse(ResultError error)
        {
            int statusCode = ResultErrorTypeToStatusCode(error.ErrorType);

            ProblemDetails problemDetails = new ProblemDetails()
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
                _ => 500,
            };
        }
    }
}
