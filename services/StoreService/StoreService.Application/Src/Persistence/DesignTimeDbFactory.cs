using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Options;
using ECommerceBackend.Utils.Database;

namespace StoreService.Application.Persistence;

public class DatabaseContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
{
    public DatabaseContext CreateDbContext(string[] args)
    {
        PostgresConfiguration cfg = new PostgresConfigurationBuilder().Build(args);
        return new DatabaseContext(Options.Create<PostgresConfiguration>(cfg));
    }
}