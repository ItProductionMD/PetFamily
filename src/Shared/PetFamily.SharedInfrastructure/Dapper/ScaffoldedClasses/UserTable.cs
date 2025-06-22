namespace PetFamily.SharedInfrastructure.Dapper.ScaffoldedClasses;

public static class UserTable
{
    public const string TableFullName = "auth.users";
    public const string Id = "id";
    public const string Login = "login";
    public const string Email = "email";
    public const string IsEmailConfirmed = "is_email_confirmed";
    public const string HashedPassword = "hashed_password";
    public const string PhoneNumber = "phone_number";
    public const string PhoneRegionCode = "phone_region_code";
    public const string Socials = "social_networks";
    public const string ProviderType = "provider_type";
    public const string IsBlocked = "is_blocked";
    public const string IsTwoFactorEnabled = "is_two_factor_enabled";
    public const string BlockedAt = "blocked_at";
}
