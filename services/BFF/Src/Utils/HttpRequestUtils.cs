using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;

namespace BFF.Utils;

public static class HttpRequestUtils
{
    public static void ApplyAuthorizationHeader(HttpRequestMessage request, HttpRequest httpRequest) =>
        ApplyAuthorizationHeader(request, httpRequest.Headers.Authorization.ToString());

    public static void ApplyAuthorizationHeader(HttpRequestMessage request, string? authorizationHeader)
    {
        if (!string.IsNullOrWhiteSpace(authorizationHeader))
        {
            request.Headers.Authorization = AuthenticationHeaderValue.Parse(authorizationHeader);
        }
    }
}
