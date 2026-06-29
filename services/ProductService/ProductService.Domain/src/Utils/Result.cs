using System.Text;

namespace ProductService.Domain.Utils;

public class Result<T>
{
    public Result(T returnResult)
    {
        ReturnResult = returnResult;
        Errors = [];
    }

    public Result(IEnumerable<ResultError> errors)
    {
        Errors = errors;
    }

    public T? ReturnResult { init; get; }
    public IEnumerable<ResultError> Errors { init; get; }

    public T GetValue()
    {
        if (Errors.Any()) throw new InvalidOperationException("Calling get value on a failed result");

        return ReturnResult!;
    }

    public string ErrorsToString()
    {
        var builder = new StringBuilder();
        foreach (var error in Errors) builder.AppendLine(error.Message);

        return builder.ToString();
    }
}