using ECommerceBackend.EventTypes;
using ECommerceBackend.Utils;
using EventSystemHelper.Kafka.Services;
using EventSystemHelper.Kafka.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProductService.Application.Persistence;
using ProductService.Domain.Entities;
using ProductService.Domain.Utils;

namespace ProductService.Application.Services;

public interface ICategoriesService
{
    public Task<List<CategoryEntity>> GetCategoriesAsync(
        string searchText,
        CancellationToken cancellationToken = default);
    public Task<Result<int>> CreateCategoryAsync(CategoryEntity entity, CancellationToken cancellationToken = default);
}

public class CategoriesService : ICategoriesService
{
    private readonly DatabaseContext _context;
    private readonly ILogger<CategoriesService> _logger;
    private readonly KafkaConfiguration _kafkaConfiguration;

    public CategoriesService(
        ILogger<CategoriesService> logger,
        DatabaseContext context,
        IOptions<KafkaConfiguration> kafkaConfiguration)
    {
        _logger = logger;
        _context = context;
        _kafkaConfiguration = kafkaConfiguration.Value;
    }

    public async Task<List<CategoryEntity>> GetCategoriesAsync(
        string searchText,
        CancellationToken cancellationToken = default)
    {
        _logger.LogTrace("Entered {FunctionName}", nameof(GetCategoriesAsync));
        _logger.LogDebug("Fetching all categories");
        IQueryable<CategoryEntity> query = _context.Categories;

        string normalizedSearchText = searchText.Trim();
        if (!string.IsNullOrWhiteSpace(normalizedSearchText))
        {
            query = query.Where(category => EF.Functions.ILike(category.Name, $"%{normalizedSearchText}%"));
        }

        List<CategoryEntity> result = await query.ToListAsync(cancellationToken);
        return result;
    }

    public async Task<Result<int>> CreateCategoryAsync(
        CategoryEntity entity,
        CancellationToken cancellationToken = default)
    {
        _logger.LogTrace("Entered CreateCategory");
        _logger.LogDebug("Creating category: {}", entity.Name);

        await _context.Categories.AddAsync(entity, cancellationToken);

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Successfully created category: {@Category}", entity);
        }
        catch (Exception ex)
        {
            _logger.LogError("An exception was thrown while saving changes: {}", ex);
            return new Result<int>([
                new ResultError(ResultErrorType.UNKNOWN_ERROR, "Failed to create the product")
            ]);
        }

        var ev = new CategoryCreatedEvent
        {
            CategoryId = entity.CategoryId,
            Name = entity.Name,
        };

        string sEvent = JsonUtils.Serialize(ev);
        await new KafkaEventProducer(_kafkaConfiguration)
            .ProduceEventAsync(ev.TopicName, sEvent, cancellationToken);

        return new Result<int>(entity.CategoryId);
    }
}
