
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Auth.Presentation.RefreshToken;

namespace PetFamily.Auth.Presentation;

public static class AuthPresentationInjector
{
    public static IServiceCollection InjectAuthPresentation(this IServiceCollection services)
    {
        services
            .AddScoped<IRefreshTokenService, RefreshTokenCookieService>()
            .AddHttpContextAccessor()
            .AddControllers();
     
        return services;
    }
}
