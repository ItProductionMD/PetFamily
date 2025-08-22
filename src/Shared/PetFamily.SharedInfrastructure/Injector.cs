using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using PetFamily.SharedApplication.Abstractions;
using PetFamily.SharedApplication.Extensions;
using PetFamily.SharedApplication.IJWTProvider;
using PetFamily.SharedApplication.IUserContext;
using PetFamily.SharedInfrastructure.HttpContext;
using PetFamily.SharedInfrastructure.JWTProvider;
using PetFamily.SharedInfrastructure.Shared.Constants;
using PetFamily.SharedInfrastructure.Shared.Dapper;
using System.Text;

namespace PetFamily.SharedInfrastructure;

public static class Injector
{
    public static IServiceCollection InjectSharedInfrastructure(
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

            .ConfigDapper(config)

            .AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = config[$"{JwtOptions.SECTION_NAME}:Issuer"],
                    ValidAudience = config[$"{JwtOptions.SECTION_NAME}:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(config[$"{JwtOptions.SECTION_NAME}:SecretKey"]!)
                    )
                };
            });

        return services;
    }
}
