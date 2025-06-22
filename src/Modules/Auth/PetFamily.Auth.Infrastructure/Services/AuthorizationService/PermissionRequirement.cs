using Microsoft.AspNetCore.Authorization;

namespace PetFamily.Auth.Infrastructure.Services.AuthorizationService;

public class PermissionRequirement : IAuthorizationRequirement
{

    public const string PERMISSION_CLAIM_TYPE = "permission";
    public string Permission { get; }

    public PermissionRequirement(string permission)
    {
        Permission = permission;
    }
}

