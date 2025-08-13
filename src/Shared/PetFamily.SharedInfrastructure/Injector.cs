using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.SharedApplication.Abstractions;
using PetFamily.SharedApplication.Extensions;
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
        var postgresConnection = configuration.TryGetConnectionString(ConnectionStringName.POSTGRESQL);
        
        return services
            .ConfigDapper(configuration)
            .AddSingleton<IDbConnectionFactory>(_ => new NpgSqlConnectionFactory(postgresConnection))
            .AddScoped<IUserContext, HTTPUserContext>();

        //.AddScoped<IUnitOfWork, UnitOfWork>();

    }
}
