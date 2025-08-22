using PetFamily.SharedKernel.Results;
using System.Security.Claims;

namespace PetFamily.SharedApplication.IJWTProvider;

public interface IJwtProvider
{
    Result<string> GetClaimValueByName(string jwtToken, string claimName);
    (string Token, DateTime ExpiresAt) GenerateJwtTokenData(
        IEnumerable<Claim> claims,
        DateTime expiresAt);

    Result<ClaimsPrincipal> ValidateToken(string token, bool validateLifetime);
}
