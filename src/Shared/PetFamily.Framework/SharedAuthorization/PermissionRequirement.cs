using Microsoft.AspNetCore.Authorization;

namespace PetFamily.Framework.SharedAuthorization;

public class PermissionRequirement : IAuthorizationRequirement
{

    public const string PERMISSION_CLAIM_TYPE = "permission";
    public string Permission { get; }

    public PermissionRequirement(string permission)
    {
        Permission = permission;
    }
}
