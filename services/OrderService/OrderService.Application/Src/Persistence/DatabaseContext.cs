using ECommerceBackend.Utils.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OrderService.Domain.Entities;

namespace OrderService.Application.Persistence;

public class DatabaseContext : DbContext
{
    private readonly IOptions<PostgresConfiguration> _postgresConfiguration;

    public DatabaseContext(IOptions<PostgresConfiguration> postgresConfiguration) =>
        _postgresConfiguration = postgresConfiguration;

    public DbSet<OrderEntity> Orders { get; set; }
    public DbSet<OrderProductEntity> OrderProducts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        PostgresConfiguration conf = _postgresConfiguration.Value;

        string conString = $@"
            Host={conf.Host};Database={conf.Database};Username={conf.Username};Password={conf.Password}
        ";

        optionsBuilder.UseNpgsql(conString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<OrderProductEntity>()
            .HasKey(x => new { x.OrderId, x.ProductId });

        modelBuilder.Entity<OrderEntity>()
            .Property(x => x.CreatedAt)
            .HasDefaultValueSql("now()");

        modelBuilder.Entity<OrderEntity>()
            .HasIndex(x => x.OrderState)
            .IsUnique();
    }
}
