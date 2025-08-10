using PetFamily.SharedInfrastructure.Constants;

namespace PetFamily.SharedInfrastructure.Shared.Dapper.ScaffoldedClassesPreview;

public static class RefreshTokensTable
{
    public const string TableName = "refresh_tokens";
    public const string TableFullName = "auth.refresh_tokens";
    public const string Id = "id";
    public const string UserId = "user_id";
    public const string Jti = "jti";
    public const string FingerPrint = "finger_print";
    public const string Token = "token";
    public const string ExpiresAt = "expires_at";
    public const string CreatedAt = "created_at";
    public const string RevokedAt = "revoked_at";
}
