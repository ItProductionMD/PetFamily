using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PetFamily.Domain.PetManagment.Entities;
using PetFamily.Domain.PetManagment.Root;
using PetFamily.Domain.Shared;
using PetFamily.Infrastructure.Constants;
using System.Reflection;


namespace PetFamily.Infrastructure.Contexts;

public class WriteDbContext : DbContext
{
    private readonly IConfiguration _configuration;
    public DbSet<Volunteer> Volunteers { get; set; }
    public DbSet<Species> AnimalTypes { get; set; }

    public WriteDbContext(IConfiguration configuration)
    {
         _configuration=configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = _configuration.GetConnectionString(ConnectionStringName.POSTGRESQL);

        optionsBuilder
            .UseNpgsql(connectionString) 
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
