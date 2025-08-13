using Microsoft.EntityFrameworkCore;
using PetFamily.Discussions.Domain.Entities;
using PetFamily.SharedInfrastructure.Constants;
using PetFamily.SharedInfrastructure.Shared.Logger;

namespace PetFamily.Discussions.Infrastructure.Contexts;

public class DiscussionWriteDbContext : DbContext
{
    public DbSet<Discussion> Discussions { get; set; }
    public DbSet<Message> Messages { get; set; }
    private readonly string _connectionString;


    public DiscussionWriteDbContext(string connectionString)
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
            .HasDefaultSchema(SchemaNames.DISCUSSION)
            .ApplyConfigurationsFromAssembly(typeof(DiscussionWriteDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
