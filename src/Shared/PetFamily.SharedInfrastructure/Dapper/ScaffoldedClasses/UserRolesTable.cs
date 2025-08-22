using PetFamily.SharedInfrastructure.Constants;

namespace PetFamily.SharedInfrastructure.Dapper.ScaffoldedClasses;

public static class UserRolesTable
{
    public const string TableName = "user_roles";
    public const string TableFullName = "\"authorization\".user_roles";
    public const string UserId = "user_id";
    public const string RoleId = "role_id";
}
