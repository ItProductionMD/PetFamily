using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace PetFamily.Auth;

public static class AuthInjector // Fixed typo in class name
{
    public static IServiceCollection InjectAuthServices(this IServiceCollection services)
    {
        var schema = JwtBearerDefaults.AuthenticationScheme;

        services.AddAuthentication(schema)
            .AddJwtBearer(schema, options =>
            {
                //options.Authority = "https://localhost:5001";
                //options.Audience = "petfamilyapi";
                //options.RequireHttpsMetadata = false;
            });

        services.AddAuthorizationBuilder()
            .AddPolicy("Admin", policy => policy.RequireRole("Admin"))
            .AddPolicy("Volunteer", policy => policy.RequireRole("Volunteer"))
            .AddPolicy("Guest", policy => policy.RequireRole("Guest"));

        return services;
    }
}
