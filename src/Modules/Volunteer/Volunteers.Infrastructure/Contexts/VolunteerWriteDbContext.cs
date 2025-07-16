using Microsoft.EntityFrameworkCore;
using PetFamily.SharedInfrastructure.Constants;
using PetFamily.SharedInfrastructure.Shared.Logger;
using VolunteerDomain = Volunteers.Domain.Volunteer;

namespace Volunteers.Infrastructure.Contexts;

public class VolunteerWriteDbContext : DbContext
{
    private readonly string _connectionString;
    public DbSet<VolunteerDomain> Volunteers { get; set; }

    public VolunteerWriteDbContext(string connectionString)
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
            .HasDefaultSchema(SchemaNames.VOLUNTEER)
            .ApplyConfigurationsFromAssembly(typeof(VolunteerWriteDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}



