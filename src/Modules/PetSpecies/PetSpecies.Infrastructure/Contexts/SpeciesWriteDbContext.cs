using Microsoft.EntityFrameworkCore;
using PetFamily.SharedInfrastructure.Shared.Logger;
using PetSpecies.Domain;
using PetFamily.SharedInfrastructure.Constants;

namespace PetSpecies.Infrastructure.Contexts;

public class SpeciesWriteDbContext : DbContext
{
    private readonly string _connectionString;
    public DbSet<Species> AnimalTypes { get; set; }

    public SpeciesWriteDbContext(string connectionString)
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
        modelBuilder.HasDefaultSchema(SchemaNames.SPECIES);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SpeciesWriteDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
