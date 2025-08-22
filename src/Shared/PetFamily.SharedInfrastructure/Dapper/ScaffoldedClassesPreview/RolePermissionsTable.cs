using PetFamily.SharedInfrastructure.Constants;

namespace PetFamily.SharedInfrastructure.Shared.Dapper.ScaffoldedClassesPreview;

public static class RolePermissionsTable
{
    public const string TableName = "role_permissions";
    public const string TableFullName = "authorization.role_permissions";
    public const string RoleId = "role_id";
    public const string PermissionId = "permission_id";
}
