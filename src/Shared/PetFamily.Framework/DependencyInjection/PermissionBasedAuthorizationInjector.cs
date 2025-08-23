using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.SharedKernel.Authorization;

namespace PetFamily.Framework.HTTPContext.AuthorizationHandler;

public static class PermissionBasedAuthorizationInjector
{
    public static IServiceCollection AddPermissionBasedAuthorizationHandler(
        this IServiceCollection services)
    {
        var permissionCodes = PermissionCodes.GetAllPermissionCodes();

        services.AddAuthorization(options =>
        {
            foreach (var permission in permissionCodes)
            {
                options.AddPolicy($"{permission}", policy =>
                    policy.Requirements.Add(new PermissionRequirement(permission)));
            }
        });

        services.AddSingleton<IAuthorizationHandler, AuthorizationByPermissionsHandler>();

        return services;
    }
}
