using Microsoft.EntityFrameworkCore;
using PetFamily.SharedInfrastructure.Constants;
using PetFamily.SharedInfrastructure.Shared.Logger;
using PetFamily.VolunteerRequests.Domain.Entities;

namespace PetFamily.VolunteerRequests.Infrastructure.Contexts;

public class VolunteerRequestDbContext : DbContext
{
    private readonly string _connectionString;
    public DbSet<VolunteerRequest> VolunteerRequests { get; set; }
    public VolunteerRequestDbContext(string connectionString)
    {
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
            .HasDefaultSchema(SchemaNames.VOLUNTEER_REQUESTS)
            .ApplyConfigurationsFromAssembly(typeof(VolunteerRequestDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
