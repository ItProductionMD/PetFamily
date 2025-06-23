using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.Abstractions;
using PetFamily.SharedApplication.IUserContext;
using PetFamily.SharedInfrastructure.HttpContext;
using PetFamily.SharedInfrastructure.Shared.Constants;
using PetFamily.SharedInfrastructure.Shared.Dapper;

namespace PetFamily.SharedInfrastructure;

public static class Injector
{
    public static IServiceCollection InjectSharedInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var postgresConnection = configuration.GetConnectionString(ConnectionStringName.POSTGRESQL);
        if (string.IsNullOrEmpty(postgresConnection))
            throw new ApplicationException("PostgreSQL connection string wasn't found!");

        return services
            .ConfigDapper(configuration)
            .AddSingleton<IDbConnectionFactory>(_ => new NpgSqlConnectionFactory(postgresConnection))
            .AddScoped<IUserContext, HttpUserContext>();

        //.AddScoped<IUnitOfWork, UnitOfWork>();

    }
}
