using System.Net.Http.Json;
using BFF.Utils;
using ECommerceBackend.Utils.Microservices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BFF.Categories;

[ApiController]
[Route("[controller]")]
public class CategoriesController(
    HttpClient httpClient,
    IOptions<MicroserviceHosts> hosts,
    ILogger<CategoriesController> logger) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetCategories(
        string searchText = "",
        CancellationToken cancellationToken = default)
    {
        string upstreamUrl = $"{hosts.Value.ProductServiceUrl}/Categories?searchText={Uri.EscapeDataString(searchText)}";
        logger.LogDebug("Fetching categories from {Url}", upstreamUrl);

        using HttpResponseMessage response = await httpClient.GetAsync(upstreamUrl, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            string body = await response.Content.ReadAsStringAsync(cancellationToken);
            logger.LogWarning(
                "Get categories failed with status {StatusCode}: {Body}",
                response.StatusCode,
                body);
            return HttpResponseUtils.FromStringBody((int)response.StatusCode, body);
        }

        List<CategoryListDto>? categories =
            await response.Content.ReadFromJsonAsync<List<CategoryListDto>>(cancellationToken);

        return Ok(new { data = categories });
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateCategory(
        [FromBody] CreateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        string upstreamUrl = $"{hosts.Value.ProductServiceUrl}/Categories";
        logger.LogDebug("Creating category at {Url}", upstreamUrl);

        using var upstreamRequest = new HttpRequestMessage(HttpMethod.Post, upstreamUrl);
        HttpRequestUtils.ApplyAuthorizationHeader(upstreamRequest, Request);

        upstreamRequest.Content = JsonContent.Create(new { name = request.Name });

        using HttpResponseMessage response = await httpClient.SendAsync(upstreamRequest, cancellationToken);
        string body = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            logger.LogWarning(
                "Create category failed with status {StatusCode}: {Body}",
                response.StatusCode,
                body);
        }

        return HttpResponseUtils.FromStringBody((int)response.StatusCode, body);
    }
}
