using PetFamily.Auth.Application.Dtos;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.ValueObjects.Ids;
using System.Security.Claims;

namespace PetFamily.Auth.Application.IServices;

public interface IJwtProvider
{
    (string AccessToken, DateTime ExpiresAt,Guid Jti) GenerateAccessTokenForPermissionRequirement(
       UserId userId,
       string login,
       string email,
       string phone,
       IEnumerable<string> roleCodes);
    (string RefreshToken, DateTime ExpiresAt) GenerateRefreshToken();

    TokenResult GenerateTokens(
        UserId userId,
        string login,
        string email,
        string phone,
        IEnumerable<string> permissionCodes);

    (string Token, DateTime ExpiresAt) GenerateJwtToken(
       IEnumerable<Claim> claims,
       DateTime issuedAt,
       DateTime expiresAt);

    Result<string> GetJtiClaim(string accessToken);

    Result<ClaimsPrincipal> ValidateToken(string token, bool validateLifetime = true);
}
