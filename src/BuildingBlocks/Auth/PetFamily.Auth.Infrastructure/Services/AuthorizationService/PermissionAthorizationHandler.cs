using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace PetFamily.Auth.Infrastructure.Services.AuthorizationService;

public class AuthorizationHandlerByPermissions : AuthorizationHandler<PermissionRequirement>
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthorizationHandlerByPermissions(IHttpContextAccessor httpContextAccessor)
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
