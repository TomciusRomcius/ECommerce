using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Options;
using ECommerceBackend.Utils.Database;

namespace UserService.Application.Persistence;

public class DatabaseContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
{
    public DatabaseContext CreateDbContext(string[] args)
    {
    string? host = args.FirstOrDefault(arg => arg.StartsWith("--host"))?.Split('=')[1];
        string? database = args.FirstOrDefault(arg => arg.StartsWith("--database"))?.Split('=')[1];
        string? username = args.FirstOrDefault(arg => arg.StartsWith("--username"))?.Split('=')[1];
        string? password = args.FirstOrDefault(arg => arg.StartsWith("--password"))?.Split('=')[1];

        PostgresConfiguration cfg = new PostgresConfigurationBuilder().Build(args);
        return new DatabaseContext(Options.Create<PostgresConfiguration>(cfg));
    }
}