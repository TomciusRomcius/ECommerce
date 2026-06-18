using ApiWorker.Persistence;
using ApiWorker.Persistence.Entities;
using ECommerceBackend.EventTypes;

namespace ApiWorker.Services;

public sealed class CategoryCreatedHandler(
    ReadDbContext readDbContext,
    ILogger<CategoryCreatedHandler> logger) : IEventHandler<CategoryCreatedEvent>
{
    public async Task HandleAsync(CategoryCreatedEvent ev, CancellationToken cancellationToken)
    {
        readDbContext.Categories.Add(new CategoryEntity(ev.CategoryId, ev.Name));
        await readDbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Upserted category {CategoryId} in read model", ev.CategoryId);
    }
}
