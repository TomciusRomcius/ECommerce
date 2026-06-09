using ECommerceBackend.Utils.Database;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Options;

namespace ApiWorker.Persistence;

public sealed class ReadDbContextFactory : IDesignTimeDbContextFactory<ReadDbContext>
{
    public ReadDbContext CreateDbContext(string[] args)
    {
        PostgresConfiguration cfg = args.Length == 0
            ? CreateDefaultConfiguration()
            : new PostgresConfigurationBuilder().Build(args);

        return new ReadDbContext(Options.Create(cfg));
    }

    private static PostgresConfiguration CreateDefaultConfiguration() =>
        new()
        {
            Host = "localhost",
            Database = "read_db",
            Username = "postgres",
            Password = "postgres",
        };
}
