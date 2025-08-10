using Microsoft.EntityFrameworkCore;
using PetFamily.Auth.Domain.Entities.RoleAggregate;
using PetFamily.Auth.Domain.Entities.UserAggregate;
using PetFamily.Auth.Domain.Entities;
using PetFamily.SharedInfrastructure.Constants;
using PetFamily.SharedInfrastructure.Shared.Logger;
using PetFamily.Auth.Infrastructure.EFCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace PetFamily.Auth.Infrastructure.Contexts;

public class AuthWriteDbContext : DbContext
{
    private readonly string _connectionString;
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }  
    public DbSet<RefreshTokenSession> RefreshTokens { get; set; }

    public AuthWriteDbContext(string connectionString)
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
        modelBuilder.HasDefaultSchema(SchemaNames.AUTH);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AuthWriteDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}


