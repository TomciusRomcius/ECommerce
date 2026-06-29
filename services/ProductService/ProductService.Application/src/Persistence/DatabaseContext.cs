using ECommerceBackend.Utils.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ProductService.Domain.Entities;

namespace ProductService.Application.Persistence;

public class DatabaseContext : DbContext
{
    public DatabaseContext(IOptions<PostgresConfiguration> postgresConfiguration)
    {
        _postgresConfiguration = postgresConfiguration;
    }

    public const int PageSize = 20;
    public DbSet<ProductEntity> Products { get; set; }
    public DbSet<ProductImageEntity> ProductImages { get; set; }
    public DbSet<CategoryEntity> Categories { get; set; }
    public DbSet<ManufacturerEntity> Manufacturers { get; set; }
    private IOptions<PostgresConfiguration> _postgresConfiguration { get; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var conf = _postgresConfiguration.Value;

        var conString = $@"
            Host={conf.Host};Database={conf.Database};Username={conf.Username};Password={conf.Password}
        ";

        optionsBuilder.UseNpgsql(conString);
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ProductImageEntity>(entity =>
        {
            entity.HasKey(e => e.ProductImageId);

            entity.Property(e => e.ProductImageId)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.S3Key)
                .HasMaxLength(36)
                .IsRequired();

            entity.HasIndex(e => e.ProductId);
            
            // Handled using Kafka. Needs to still be persisted to delete S3 objects
            entity.HasOne(e => e.Product)
                .WithMany(p => p.Images)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();
        });
    }
}