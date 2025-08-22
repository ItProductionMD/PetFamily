using Authorization.Application.Dtos;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.ValueObjects.Ids;
using System.Security.Claims;

namespace Authorization.Application.IAuthorizationToken;

public interface IAuthorizationTokenProvider
{
    Result<string> GetClaimValueByName(string accessToken, string claimName);
    (string RefreshToken, DateTime ExpiresAt) GenerateRefreshTokenData();
    (string AccessToken, DateTime ExpiresAt) GenerateAccessTokenData(IEnumerable<Claim> claims);
    Result<ClaimsPrincipal> ValidateAccessToken(string token, bool validateLifetime = true);
}

