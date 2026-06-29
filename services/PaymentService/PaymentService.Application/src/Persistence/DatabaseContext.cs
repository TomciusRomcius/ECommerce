using ECommerceBackend.Utils.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PaymentService.Domain.src.Entities;

namespace PaymentService.Application.src.Persistence
{
    public class DatabaseContext : DbContext
    {
        public DbSet<PaymentSessionEntity> PaymentSessions { get; set; }
        private PostgresConfiguration _postgresConfiguration;

        public DatabaseContext(IOptions<PostgresConfiguration> postgresConfiguration)
        {
            _postgresConfiguration = postgresConfiguration.Value;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string host = _postgresConfiguration.Host;
            string database = _postgresConfiguration.Database;
            string username = _postgresConfiguration.Username;
            string password = _postgresConfiguration.Password;
            optionsBuilder.UseNpgsql($"Host={host};Database={database};Username={username};Password={password}");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PaymentSessionEntity>()
                .HasKey(ps => ps.PaymentSessionId);
            modelBuilder.Entity<PaymentSessionEntity>()
                .HasIndex(ps => ps.UserId)
                .IsUnique();
            modelBuilder.Entity<PaymentSessionEntity>()
                .Property(ps => ps.PaymentSessionProvider)
                .IsRequired();
        }
    }
}
