using PetFamily.SharedInfrastructure.Constants;

namespace PetFamily.SharedInfrastructure.Dapper.ScaffoldedClasses;

public static class UserRoleTable 
{
    public const string TableName = "user_role";
    public const string TableFullName = "auth.user_role";
    public const string UserId = "user_id";
    public const string RoleId = "role_id";
    public const string DeletedAt = "deleted_at";
    public const string IsDeleted = "is_deleted";
}
