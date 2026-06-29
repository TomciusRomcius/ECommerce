using ApiWorker.Persistence;
using ECommerceBackend.EventTypes;
using Microsoft.EntityFrameworkCore;

namespace ApiWorker.Services;

public sealed class CategoryUpdatedHandler(
    ReadDbContext readDbContext,
    ILogger<CategoryUpdatedHandler> logger) : IEventHandler<CategoryUpdatedEvent>
{
    public async Task HandleAsync(CategoryUpdatedEvent ev, CancellationToken cancellationToken)
    {
        int rowsAffected = await readDbContext.Categories
            .Where(category => category.CategoryId == ev.CategoryId)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(category => category.Name, ev.Name),
                cancellationToken);

        if (rowsAffected == 0)
        {
            logger.LogWarning(
                "Category {CategoryId} not found in read model for update",
                ev.CategoryId);
            return;
        }

        await readDbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Updated category {CategoryId} in read model", ev.CategoryId);
    }
}
