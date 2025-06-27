using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using PetFamily.Auth.Infrastructure.Services.JwtProvider;
using PetFamily.SharedApplication.Extensions;
using System.Text;

namespace PetFamily.Auth.Infrastructure.AuthInjector;

public static class JwtAuthenticationInjector
{
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration config)
    {
        config.CheckSectionsExistence([JwtOptions.SECTION_NAME]);

        services.AddAuthentication("Bearer")
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
