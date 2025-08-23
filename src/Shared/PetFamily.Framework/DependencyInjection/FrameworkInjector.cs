using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Framework.HTTPContext.AuthorizationHandler;
using PetFamily.Framework.HTTPContext.Cookie;
using PetFamily.Framework.HTTPContext.User;
using PetFamily.SharedApplication.DependencyInjection;

namespace PetFamily.Framework.DependencyInjection;

public static class FrameworkInjector
{
    public static IServiceCollection AddFramework(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var refreshTokenSection = RefreshTokenCookieOptions.SECTION_NAME;

        configuration.CheckSectionsExistence([refreshTokenSection]);

        services
            .Configure<RefreshTokenCookieOptions>(configuration.GetSection(refreshTokenSection));

        services
            .AddPermissionBasedAuthorizationHandler()
            .AddJwtBearerAuthentication(configuration)
            .AddScoped<IUserContext, UserContext>()
            .AddScoped<ICookieService, CookieService>();

        return services;
    }
}
