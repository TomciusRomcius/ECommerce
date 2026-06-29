using Microsoft.AspNetCore.Mvc;

namespace BFF.Utils;

public static class HttpResponseUtils
{
    public static ContentResult FromStringBody(int statusCode, string? body = null) =>
        new()
        {
            StatusCode = statusCode,
            Content = body,
            ContentType = "application/json",
        };
}
