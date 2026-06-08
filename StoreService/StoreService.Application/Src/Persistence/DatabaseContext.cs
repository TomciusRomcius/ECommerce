using ECommerceBackend.Utils.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using StoreService.Domain.Entities;

namespace StoreService.Application.Persistence;

public class DatabaseContext : DbContext
{
    public const int PageSize = 20;
    private IOptions<PostgresConfiguration> _postgresConfiguration;

    public DatabaseContext(IOptions<PostgresConfiguration> postgresConfiguration)
        : base()
    {
        _postgresConfiguration = postgresConfiguration;
    }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var conf = _postgresConfiguration.Value;

        var conString = $@"
            Host={conf.Host};Database={conf.Database};Username={conf.Username};Password={conf.Password}
        ";

        optionsBuilder.UseNpgsql(conString);
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProductStoreLocationEntity>()
            .HasKey(psl => new { psl.StoreLocationId, psl.ProductId });

        modelBuilder.Entity<ReservedProductEntity>()
            .HasKey(rp => new { rp.OrderId, rp.StoreLocationId, rp.ProductId });

        modelBuilder.Entity<ReservedProductEntity>()
            .HasOne(rp => rp.StoreLocation)
            .WithOne()
            .HasForeignKey<ReservedProductEntity>(rp => rp.StoreLocationId);

        base.OnModelCreating(modelBuilder);
    }

    public DbSet<StoreLocationEntity> StoreLocations { get; set; }
    public DbSet<ProductStoreLocationEntity> ProductStoreLocations { get; set; }
    public DbSet<ReservedProductEntity> ReservedProducts { get; set; }
}