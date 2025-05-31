using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.SharedInfrastructure.Shared.Constants;
using PetFamily.SharedInfrastructure.Shared.EFCore;
using PetSpecies.Application;
using PetSpecies.Application.IRepositories;
using PetSpecies.Infrastructure.Contexts;
using PetSpecies.Infrastructure.Contracts;
using PetSpecies.Infrastructure.EFCore;
using PetSpecies.Infrastructure.Repositories.Read;
using PetSpecies.Infrastructure.Repositories.Write;
using PetSpecies.Public.IContracts;

namespace PetSpecies.Infrastructure;

public static class SpeciesModuleInjector
{
    public static IServiceCollection InjectSpeciesModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var postgresConnection = configuration.GetConnectionString(ConnectionStringName.POSTGRESQL);
        if (string.IsNullOrEmpty(postgresConnection))
            throw new ApplicationException("PostgreSQL connection string wasn't found!");


        services.InjectSpeciesApplication(configuration);

        services.AddScoped<IDbMigrator, MigratorForSpecies>();

        services.AddScoped<ISpeciesReadRepository, SpeciesReadRepository>()
                .AddScoped<ISpeciesWriteRepository, SpeciesWriteRepository>();

        services.AddScoped<SpeciesWriteDbContext>(_ => new SpeciesWriteDbContext(postgresConnection));

        services.AddScoped<ISpeciesExistenceContract, SpeciesExistenceContract>()
                .AddScoped<ISpeciesQueryContract, SpeciesQueryContract>();

        return services;
    }
}
