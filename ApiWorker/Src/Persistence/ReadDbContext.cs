using ApiWorker.Persistence.Entities;
using ECommerceBackend.Utils.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ApiWorker.Persistence;

public sealed class ReadDbContext : DbContext
{
    private readonly IOptions<PostgresConfiguration> _postgresConfiguration;

    public ReadDbContext(IOptions<PostgresConfiguration> postgresConfiguration)
    {
        _postgresConfiguration = postgresConfiguration;
    }

    public DbSet<StoreProductReadEntity> StoreProducts { get; set; }

    public DbSet<StoreLocationEntity> StoreLocations { get; set; }

    public DbSet<ProductEntity> Products { get; set; }

    public DbSet<ManufacturerEntity> Manufacturers { get; set; }

    public DbSet<CategoryEntity> Categories { get; set; }

    public DbSet<ProductImageReadEntity> ProductImages { get; set; }

    public DbSet<ProcessedMessageEntity> ProcessedMessages { get; set; }

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
            entity.HasKey(storeProduct => new { storeProduct.StoreLocationId, storeProduct.ProductId });

            entity.HasIndex(storeProduct => storeProduct.ProductId);
        });

        modelBuilder.Entity<StoreLocationEntity>(entity =>
        {
            entity.HasKey(storeLocation => storeLocation.StoreLocationId);

            entity.Property(storeLocation => storeLocation.StoreLocationId)
                .ValueGeneratedNever();
        });

        modelBuilder.Entity<ManufacturerEntity>(entity =>
        {
            entity.HasKey(manufacturer => manufacturer.ManufacturerId);

            entity.Property(manufacturer => manufacturer.ManufacturerId)
                .ValueGeneratedNever();
        });

        modelBuilder.Entity<CategoryEntity>(entity =>
        {
            entity.HasKey(category => category.CategoryId);

            entity.Property(category => category.CategoryId)
                .ValueGeneratedNever();
        });

        modelBuilder.Entity<ProductEntity>(entity =>
        {
            entity.HasKey(product => product.ProductId);

            entity.Property(product => product.ProductId)
                .ValueGeneratedNever();

            entity.HasIndex(product => product.CategoryId);
            entity.HasIndex(product => product.ManufacturerId);
        });

        modelBuilder.Entity<ProductImageReadEntity>(entity =>
        {
            entity.HasKey(image => image.ProductImageId);

            entity.Property(image => image.ProductImageId)
                .ValueGeneratedNever();

            entity.Property(image => image.S3Key)
                .HasMaxLength(36)
                .IsRequired();

            entity.HasIndex(image => image.ProductId);
        });

        modelBuilder.Entity<ProcessedMessageEntity>(entity =>
        {
            entity.HasKey(message => message.Id);

            entity.Property(message => message.Id)
                .ValueGeneratedOnAdd();

            entity.Property(message => message.MessageId)
                .HasMaxLength(36)
                .IsRequired();

            entity.HasIndex(message => message.MessageId)
                .IsUnique();
        });

        base.OnModelCreating(modelBuilder);
    }
}
