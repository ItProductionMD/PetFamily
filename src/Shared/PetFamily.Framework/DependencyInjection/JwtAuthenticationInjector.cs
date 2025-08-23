using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using PetFamily.SharedInfrastructure.JWTProvider;
using System.Text;


namespace PetFamily.Framework.DependencyInjection;

public static class JwtAuthenticationInjector
{
    public static IServiceCollection AddJwtBearerAuthentication(
        this IServiceCollection services, IConfiguration config)
    {
        services
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
                        Encoding.UTF8.GetBytes(
                            config[$"{JwtOptions.SECTION_NAME}:SecretKey"]!
                        )
                    )
                };
            });

        return services;
    }
}
