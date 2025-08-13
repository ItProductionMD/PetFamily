using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.SharedKernel.Authorization;

namespace PetFamily.Framework.SharedAuthorization;

public static class PermissionsPolicesAuthorizationInjector
{
    public static IServiceCollection InjectPermissionPoliciesAuthorization(
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
