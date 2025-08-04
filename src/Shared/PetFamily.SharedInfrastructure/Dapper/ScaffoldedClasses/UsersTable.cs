using PetFamily.SharedInfrastructure.Constants;

namespace PetFamily.SharedInfrastructure.Dapper.ScaffoldedClasses;

public static class UsersTable 
{
    public const string TableName = "users";
    public const string TableFullName = "auth.users";
    public const string Id = "id";
    public const string Login = "login";
    public const string HashedPassword = "hashed_password";
    public const string ProviderType = "provider_type";
    public const string UserStatus = "user_status";
    public const string IsEmailConfirmed = "is_email_confirmed";
    public const string Email = "email";
    public const string IsTwoFactorEnabled = "is_two_factor_enabled";
    public const string PhoneNumber = "phone_number";
    public const string PhoneRegionCode = "phone_region_code";
    public const string BlockedAt = "blocked_at";
    public const string CreatedAt = "created_at";
    public const string DeletedAt = "deleted_at";
    public const string IsBlocked = "is_blocked";
    public const string LastLoginDate = "last_login_date";
    public const string LoginAttempts = "login_attempts";
    public const string UpdatedAt = "updated_at";
    public const string SocialNetworks = "social_networks";
    public const string IsDeleted = "is_deleted";
}
