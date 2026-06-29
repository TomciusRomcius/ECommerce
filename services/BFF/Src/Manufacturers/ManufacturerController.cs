using System.Net.Http.Json;
using BFF.Utils;
using ECommerceBackend.Utils.Microservices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BFF.Manufacturers;

[ApiController]
[Route("[controller]")]
public class ManufacturerController(
    HttpClient httpClient,
    IOptions<MicroserviceHosts> hosts,
    ILogger<ManufacturerController> logger) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetManufacturers(
        string searchText = "",
        CancellationToken cancellationToken = default)
    {
        string upstreamUrl = $"{hosts.Value.ProductServiceUrl}/Manufacturer?searchText={Uri.EscapeDataString(searchText)}";
        logger.LogDebug("Fetching manufacturers from {Url}", upstreamUrl);

        using HttpResponseMessage response = await httpClient.GetAsync(upstreamUrl, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            string body = await response.Content.ReadAsStringAsync(cancellationToken);
            logger.LogWarning(
                "Get manufacturers failed with status {StatusCode}: {Body}",
                response.StatusCode,
                body);
            return HttpResponseUtils.FromStringBody((int)response.StatusCode, body);
        }

        List<ManufacturerListDto>? manufacturers =
            await response.Content.ReadFromJsonAsync<List<ManufacturerListDto>>(cancellationToken);

        return Ok(new { data = manufacturers });
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateManufacturer(
        [FromBody] CreateManufacturerRequest request,
        CancellationToken cancellationToken)
    {
        string upstreamUrl = $"{hosts.Value.ProductServiceUrl}/Manufacturer";
        logger.LogDebug("Creating manufacturer at {Url}", upstreamUrl);

        using var upstreamRequest = new HttpRequestMessage(HttpMethod.Post, upstreamUrl);
        HttpRequestUtils.ApplyAuthorizationHeader(upstreamRequest, Request);

        upstreamRequest.Content = JsonContent.Create(new { name = request.Name });

        using HttpResponseMessage response = await httpClient.SendAsync(upstreamRequest, cancellationToken);
        string body = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            logger.LogWarning(
                "Create manufacturer failed with status {StatusCode}: {Body}",
                response.StatusCode,
                body);
        }

        return HttpResponseUtils.FromStringBody((int)response.StatusCode, body);
    }
}
