using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.SharedInfrastructure.Shared.Constants;
using Volunteers.Application;
using Volunteers.Application.IRepositories;
using Volunteers.Infrastructure.BackgroundServices;
using Volunteers.Infrastructure.Contexts;
using Volunteers.Infrastructure.Contracts;
using Volunteers.Infrastructure.Dapper;
using Volunteers.Infrastructure.Repositories;
using Volunteers.Public.IContracts;

namespace Volunteers.Infrastructure;

public static class VolunteerModuleInjector
{
    public static IServiceCollection InjectVolunteerModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var postgresConnection = configuration.GetConnectionString(ConnectionStringName.POSTGRESQL);
        if (string.IsNullOrEmpty(postgresConnection))
            throw new ApplicationException("PostgreSQL connection string wasn't found!");

        services.InjectVolunteerApplication(configuration);

        services.Configure<PetImagesValidatorOptions>(configuration.GetSection("FileValidators:PetImages"));

        services
            .AddScoped<IVolunteerReadRepository, VolunteerReadRepository>()
            .AddScoped<IVolunteerWriteRepository, VolunteerWriteRepository>()
            .AddScoped<VolunteerWriteDbContext>(_ => new VolunteerWriteDbContext(postgresConnection));

        services.AddHostedService<VolunteerSoftDeletableCleanupService>();

        services.AddScoped<IPetExistenceContract, PetExistenceContract>();

        VolunteerDapperConvertor.Register();

        return services;
    }
}
