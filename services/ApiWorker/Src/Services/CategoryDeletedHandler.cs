using ApiWorker.Persistence;
using ECommerceBackend.EventTypes;
using Microsoft.EntityFrameworkCore;

namespace ApiWorker.Services;

public sealed class CategoryDeletedHandler(
    ReadDbContext readDbContext,
    ILogger<CategoryDeletedHandler> logger) : IEventHandler<CategoryDeletedEvent>
{
    public async Task HandleAsync(CategoryDeletedEvent ev, CancellationToken cancellationToken)
    {
        bool exists = await readDbContext.Categories
            .AnyAsync(category => category.CategoryId == ev.CategoryId, cancellationToken);

        if (!exists)
        {
            logger.LogWarning("Category {CategoryId} not found in read model for deletion", ev.CategoryId);
            return;
        }

        await readDbContext.Categories
            .Where(category => category.CategoryId == ev.CategoryId)
            .ExecuteDeleteAsync(cancellationToken);

        logger.LogInformation("Deleted category {CategoryId} from read model", ev.CategoryId);
    }
}
