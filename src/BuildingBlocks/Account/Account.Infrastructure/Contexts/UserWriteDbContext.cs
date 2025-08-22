using Microsoft.EntityFrameworkCore;
using Account.Domain.Entities;
using Account.Domain.Entities.UserAggregate;
using PetFamily.SharedInfrastructure.Constants;
using PetFamily.SharedInfrastructure.Shared.Logger;

namespace Account.Infrastructure.Contexts;

public class UserWriteDbContext : DbContext
{
    public const string SCHEMA_NAME = "user_account";
    private readonly string _connectionString;
    public DbSet<User> Users { get; set; }

    public UserWriteDbContext(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Connection string cannot be null or empty.", nameof(connectionString));

        _connectionString = connectionString;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseNpgsql(_connectionString)
            .UseSnakeCaseNamingConvention()
            .UseLoggerFactory(MyLoggerFactory.LoggerFactoryInstance)
            .EnableSensitiveDataLogging();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(SCHEMA_NAME);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserWriteDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}


