using Microsoft.EntityFrameworkCore;
using PetFamily.SharedInfrastructure.Constants;
using PetFamily.SharedInfrastructure.Shared.Logger;
using PetFamily.VolunteerRequests.Domain.Entities;

namespace PetFamily.VolunteerRequests.Infrastructure.Contexts;

public class VolunteerRequestWriteDbContext : DbContext
{
    private readonly string _connectionString;
    public DbSet<VolunteerRequest> VolunteerRequests { get; set; }
    public VolunteerRequestWriteDbContext(string connectionString)
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
        modelBuilder
            .HasDefaultSchema(SchemaNames.VOLUNTEER_REQUEST)
            .ApplyConfigurationsFromAssembly(typeof(VolunteerRequestWriteDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
