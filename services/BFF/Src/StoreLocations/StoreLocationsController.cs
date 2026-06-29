using AutoMapper;
using BFF.ReadDb;
using BFF.ReadDb.Entities;
using BFF.Utils;
using ECommerceBackend.Utils.Microservices;
using ECommerceBackend.Utils.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BFF.StoreLocations;

public class StoreLocationDto
{
    public int StoreLocationId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}

[ApiController]
[Route("[controller]")]
public class StoreLocationsController(
    ILogger<StoreLocationsController> logger,
    HttpClient httpClient,
    ReadDbContext readDbContext,
    IOptions<MicroserviceHosts> hosts,
    IMapper mapper) : ControllerBase
{
    private const int PageSize = 20;

    [HttpGet]
    public async Task<IActionResult> GetStoreLocations(
        [FromQuery] int page,
        CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Fetching store locations from ReadDB, page {Page}", page);

        var locations = await readDbContext.StoreLocations
            .AsNoTracking()
            .OrderBy(storeLocation => storeLocation.StoreLocationId)
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .ToPageAsync(page, PageSize);

        return Ok(new { data = mapper.MapPage<StoreLocationEntity, StoreLocationDto>(locations) });
    }

    [HttpGet("{storeLocationId:int}")]
    public async Task<IActionResult> GetStoreLocation(
        int storeLocationId,
        CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Fetching store location {StoreLocationId} from ReadDB", storeLocationId);

        StoreLocationEntity? location = await readDbContext.StoreLocations
            .AsNoTracking()
            .FirstOrDefaultAsync(
                storeLocation => storeLocation.StoreLocationId == storeLocationId,
                cancellationToken);

        if (location is null)
        {
            return NotFound();
        }

        return Ok(new
        {
            data = new StoreLocationDto
            {
                StoreLocationId = location.StoreLocationId,
                DisplayName = location.DisplayName,
                Address = location.Address,
            },
        });
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateStoreLocation(
        [FromBody] CreateStoreLocationRequest request,
        CancellationToken cancellationToken)
    {
        string upstreamUrl = $"{hosts.Value.StoreServiceUrl}/storelocation";

        using var upstreamRequest = new HttpRequestMessage(HttpMethod.Post, upstreamUrl);
        HttpRequestUtils.ApplyAuthorizationHeader(upstreamRequest, Request);

        upstreamRequest.Content = JsonContent.Create(new
        {
            displayName = request.DisplayName,
            address = request.Address,
        });

        using HttpResponseMessage response = await httpClient.SendAsync(upstreamRequest, cancellationToken);
        string body = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            logger.LogWarning(
                "Create store location failed with status {StatusCode}: {Body}",
                response.StatusCode,
                body);
        }

        return HttpResponseUtils.FromStringBody((int)response.StatusCode, body);
    }
}
