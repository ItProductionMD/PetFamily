using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PetFamily.Domain.PetManagment.Root;
using PetFamily.Domain.PetTypeManagment.Root;
using PetFamily.Domain.Shared;
using PetFamily.Infrastructure.Constants;
using System.Reflection;


namespace PetFamily.Infrastructure.Contexts;

public class WriteDbContext : DbContext
{
    private readonly string _connectionString;
    public DbSet<Volunteer> Volunteers { get; set; }
    public DbSet<Species> AnimalTypes { get; set; }

    public WriteDbContext(string connectionString)
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
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WriteDbContext).Assembly);

        base.OnModelCreating(modelBuilder); 
    }

}
