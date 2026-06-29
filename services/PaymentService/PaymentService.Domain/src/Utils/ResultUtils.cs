using FluentValidation.Results;

namespace PaymentService.Domain.src.Utils
{
    public static class ResultUtils
    {
        public static List<ResultError> ValidationFailuresToResultErrors(List<ValidationFailure> errors)
        {
            return errors.Select(
                error => new ResultError(ResultErrorType.VALIDATION_ERROR, error.ErrorMessage)
            ).ToList();
        }

        /// <summary>
        /// Errors list must not be empty, otherwise an exception will be thrown
        /// </summary>
        public static ResultError ValidationFailuresToResultError(List<ValidationFailure> errors)
        {
            return new ResultError(ResultErrorType.VALIDATION_ERROR, errors.First().ErrorMessage);
        }
    }
}
