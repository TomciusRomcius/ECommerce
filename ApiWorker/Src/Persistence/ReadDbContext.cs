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
        modelBuilder.Entity<StoreProductReadEntity>()
            .HasKey(entity => new { entity.StoreLocationId, entity.ProductId });

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
