using Authorization.Application.Dtos;
using Authorization.Application.IAuthorizationToken;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PetFamily.SharedApplication.IJWTProvider;
using PetFamily.SharedInfrastructure.JWTProvider;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.ValueObjects.Ids;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Authorization.Infrastructure.AuthorizationToken;

public class AuthorizationTokenProvider : IAuthorizationTokenProvider
{
    private readonly JwtOptions _jwtOptions;
    private readonly byte[] _secret;
    private readonly IJwtProvider _jwtProvider;

    public AuthorizationTokenProvider(IOptions<JwtOptions> jwtOptions, IJwtProvider jwtProvider)
    {
        _jwtOptions = jwtOptions.Value;
        _secret = Encoding.UTF8.GetBytes(_jwtOptions.SecretKey);
        _jwtProvider = jwtProvider;
    }

    public Result<string> GetClaimValueByName(string accessToken, string claimName)
    {
        var result = _jwtProvider.GetClaimValueByName(accessToken, claimName);
        if (result.IsSuccess) 
            return result;
        
        return Result.Fail(Error.Authentication($"Access token get claim :{claimName} error!"));
    }

    public (string AccessToken, DateTime ExpiresAt) GenerateAccessTokenData(IEnumerable<Claim> claims)
    {
        var expiresAt = DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenLifetimeMinutes);
        return _jwtProvider.GenerateJwtTokenData(claims, expiresAt);
    }

    public (string RefreshToken, DateTime ExpiresAt) GenerateRefreshTokenData()
    {
        var randomNumber = new byte[64];

        using var randomGenerator = RandomNumberGenerator.Create();

        randomGenerator.GetBytes(randomNumber);

        var refreshToken = Convert.ToBase64String(randomNumber);

        var expires = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenLifetimeDays);

        return (refreshToken, expires);
    }

    public Result<ClaimsPrincipal> ValidateAccessToken(string token, bool validateLifetime = true)
    {
        return _jwtProvider.ValidateToken(token, validateLifetime);
    }
}

