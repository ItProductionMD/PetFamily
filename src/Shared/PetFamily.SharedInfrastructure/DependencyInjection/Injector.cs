using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using PetFamily.SharedApplication.Abstractions;
using PetFamily.SharedApplication.DependencyInjection;
using PetFamily.SharedApplication.IJWTProvider;
using PetFamily.SharedInfrastructure.JWTProvider;
using PetFamily.SharedInfrastructure.Shared.Constants;
using PetFamily.SharedInfrastructure.Shared.Dapper;
using System.Text;

namespace PetFamily.SharedInfrastructure.DependencyInjection;

public static class Injector
{
    public static IServiceCollection AddSharedInfrastructure(
        this IServiceCollection services,
        IConfiguration config)
    {
        var postgresConnection = config.TryGetConnectionString(ConnectionStringName.POSTGRESQL);

        config.CheckSectionsExistence([JwtOptions.SECTION_NAME]);
        services.Configure<JwtOptions>(config.GetSection(JwtOptions.SECTION_NAME));

        services
            //.AddCustomOptions<JwtOptions>(config)

            .AddScoped<IJwtProvider, JwtProvider>()
            .AddSingleton<IDbConnectionFactory>(_ => new NpgSqlConnectionFactory(postgresConnection))
            .ConfigDapper(config);
            
        return services;
    }
}
