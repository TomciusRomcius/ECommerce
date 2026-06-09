using BFF.ReadDb.Entities;
using ECommerceBackend.Utils.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BFF.ReadDb;

public sealed class ReadDbContext : DbContext
{
    private readonly IOptions<PostgresConfiguration> _postgresConfiguration;

    public ReadDbContext(IOptions<PostgresConfiguration> postgresConfiguration)
    {
        _postgresConfiguration = postgresConfiguration;
    }

    public DbSet<StoreProductReadEntity> StoreProducts { get; set; }

    public DbSet<ProductImageReadEntity> ProductImages { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        PostgresConfiguration conf = _postgresConfiguration.Value;

        string conString = $@"
            Host={conf.Host};Database={conf.Database};Username={conf.Username};Password={conf.Password}
        ";

        optionsBuilder.UseNpgsql(conString);
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StoreProductReadEntity>(entity =>
        {
            entity.HasKey(product => new { product.StoreLocationId, product.ProductId });

            entity.HasMany(product => product.ProductImages)
                .WithOne()
                .HasForeignKey(image => image.ProductId)
                .HasPrincipalKey(product => product.ProductId);
        });

        modelBuilder.Entity<ProductImageReadEntity>(entity =>
        {
            entity.HasKey(image => image.ProductImageId);

            entity.Property(image => image.ProductImageId)
                .ValueGeneratedOnAdd();

            entity.Property(image => image.S3Key)
                .HasMaxLength(36)
                .IsRequired();

            entity.HasIndex(image => image.ProductId);
        });

        base.OnModelCreating(modelBuilder);
    }
}
