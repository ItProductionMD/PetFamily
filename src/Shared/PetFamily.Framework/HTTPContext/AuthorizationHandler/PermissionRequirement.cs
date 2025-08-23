using Microsoft.AspNetCore.Authorization;

namespace PetFamily.Framework.HTTPContext.AuthorizationHandler;

public class PermissionRequirement : IAuthorizationRequirement
{

    public const string PERMISSION_CLAIM_TYPE = "permission";
    public string Permission { get; }

    public PermissionRequirement(string permission)
    {
        Permission = permission;
    }
}
