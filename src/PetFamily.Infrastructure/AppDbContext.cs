using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PetFamily.Domain.PetManagment.Entities;
using PetFamily.Domain.PetManagment.Root;
using static PetFamily.Domain.Shared.Constants;


namespace PetFamily.Infrastructure;

public class AppDbContext : DbContext
{
    private readonly IConfiguration _configuration;
    public DbSet<Volunteer> Volunteers { get; set; }
    public DbSet<Species> AnimalTypes { get; set; }

    public AppDbContext(IConfiguration configuration)
    {
         _configuration=configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = _configuration.GetConnectionString(POSTGRE_CONNECTION_NAME);

        optionsBuilder
            .UseNpgsql(connectionString) 
            .UseSnakeCaseNamingConvention()
            .UseLoggerFactory(MyLoggerFactory.LoggerFactoryInstance)
            .EnableSensitiveDataLogging();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        base.OnModelCreating(modelBuilder); 
    }
}
