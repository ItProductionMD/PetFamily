using Authorization.Domain.Entities;
using Authorization.Infrastructure.EFCore;
using Microsoft.EntityFrameworkCore;
using PetFamily.SharedInfrastructure.Shared.Logger;

namespace Authorization.Infrastructure.Contexts;

public class JwtTokenWriteDbContext : DbContext
{
    public const string SCHEMA_NAME = "jwt_token";
    private readonly string _connectionString;
    public DbSet<RefreshTokenSession> RefreshTokens { get; set; }

    public JwtTokenWriteDbContext(string connectionString)
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

        modelBuilder.ApplyConfiguration(new RefreshTokenSessionConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}
