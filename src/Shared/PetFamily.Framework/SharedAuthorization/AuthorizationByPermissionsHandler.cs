using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace PetFamily.Framework.SharedAuthorization;

public class AuthorizationByPermissionsHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthorizationByPermissionsHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        var user = context.User;
        if (!user.Identity?.IsAuthenticated ?? true)
            return Task.CompletedTask;

        if (user.HasClaim(PermissionRequirement.PERMISSION_CLAIM_TYPE, requirement.Permission))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        return Task.CompletedTask;
    }

}

