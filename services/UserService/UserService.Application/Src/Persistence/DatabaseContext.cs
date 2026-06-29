using ECommerceBackend.Utils.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using UserService.Domain.Entities;

namespace UserService.Application.Persistence;

public class DatabaseContext : DbContext
{
    private readonly IOptions<PostgresConfiguration> _postgresConfiguration;
    public DbSet<CartProductEntity> CartProducts { get; set; }
    public DbSet<ShippingAddressEntity> ShippingAddresses { get; set; }

    public DatabaseContext(IOptions<PostgresConfiguration> postgresConfiguration)
    {
        _postgresConfiguration = postgresConfiguration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        PostgresConfiguration conf = _postgresConfiguration.Value;

        var conString = $@"
            Host={conf.Host};Database={conf.Database};Username={conf.Username};Password={conf.Password}
        ";

        optionsBuilder.UseNpgsql(conString);
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<CartProductEntity>()
            .HasKey(cp => new { cp.UserId, cp.StoreLocationId, cp.ProductId });

        base.OnModelCreating(builder);
    }
}