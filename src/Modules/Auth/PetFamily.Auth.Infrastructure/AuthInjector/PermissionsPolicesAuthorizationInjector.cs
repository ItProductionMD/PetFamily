using Microsoft.Extensions.DependencyInjection;
using PetFamily.Auth.Domain.Constants;
using PetFamily.Auth.Infrastructure.Services.AuthorizationService;
using System.Security.Claims;
using PetFamily.SharedKernel.Authorization;

namespace PetFamily.Auth.Infrastructure.AuthInjector;

public static class PermissionsPolicesAuthorizationInjector
{
    public static IServiceCollection AddPermissionPoliciesAuthorization(
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
        return services;
    }
}
