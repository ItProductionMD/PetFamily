using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PetFamily.Auth.Application.Dtos;
using PetFamily.Auth.Application.IServices;
using PetFamily.Framework.SharedAuthorization;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.ValueObjects.Ids;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PetFamily.Auth.Infrastructure.Services.JwtProvider;

public class JwtProvider : IJwtProvider
{
    private readonly JwtOptions _jwtOptions;
    private readonly byte[] _secret;

    public JwtProvider(IOptions<JwtOptions> jwtOptions)
    {
        _jwtOptions = jwtOptions.Value;
        _secret = Encoding.UTF8.GetBytes(_jwtOptions.SecretKey);
    }

    public Result<string> GetJtiClaim(string accessToken)
    {
        var claimsResult = ValidateToken(accessToken, false);
        if(claimsResult.IsFailure)
            return Result.Fail(claimsResult.Error);

        var claims = claimsResult.Data;
        var jti = claims?.FindFirst(JwtRegisteredClaimNames.Jti);
        if (jti != null && !string.IsNullOrWhiteSpace(jti.Value))
            return Result.Ok(jti.Value);

        return Result.Fail(Error.Authentication("Access token jti Error"));
    }

    public (string AccessToken, DateTime ExpiresAt,Guid Jti) GenerateAccessTokenForPermissionRequirement(
       UserId userId,
       string login,
       string email,
       string phone,
       IEnumerable<string> permissionCodes)
    {
        var now = DateTime.UtcNow;

        var jti = Guid.NewGuid();

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.Value.ToString()),
            new Claim(ClaimTypes.Name, login),
            new Claim(JwtRegisteredClaimNames.Jti, jti.ToString()),
        };
        if (!string.IsNullOrWhiteSpace(phone))
        {
            claims.Add(new Claim(ClaimTypes.MobilePhone, phone));
        }
        if (!string.IsNullOrWhiteSpace(email))
        {
            claims.Add(new Claim(ClaimTypes.Email, email ));
        }
        claims.AddRange(permissionCodes.Select(permissionCode =>
            new Claim(PermissionRequirement.PERMISSION_CLAIM_TYPE, permissionCode)));

        var accessToken = GenerateJwtToken(
            claims,
            now,
            now.AddMinutes(_jwtOptions.AccessTokenLifetimeMinutes));

        return (accessToken.Token, accessToken.ExpiresAt, jti);
    }

    public (string RefreshToken, DateTime ExpiresAt) GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        var refreshToken =  Convert.ToBase64String(randomNumber);
        var expires = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenLifetimeDays);
        return (refreshToken, expires);
    }

    public TokenResult GenerateTokens(
        UserId userId,
        string login,
        string email,
        string phone,
        IEnumerable<string> roleCodes)
    {
        var accessToken = GenerateAccessTokenForPermissionRequirement(userId, login, email, phone, roleCodes);

        var refreshToken = GenerateRefreshToken();

        var result = new TokenResult(
            AccessToken: accessToken.AccessToken,
            AccessTokenExpiresAt: accessToken.ExpiresAt,
            RefreshToken: refreshToken.RefreshToken,
            RefreshTokenExpiresAt: refreshToken.ExpiresAt,
            Jti: accessToken.Jti);

        return result;
    }

    public (string Token, DateTime ExpiresAt) GenerateJwtToken(
        IEnumerable<Claim> claims, 
        DateTime issuedAt,
        DateTime expiresAt)
    {
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(_secret), 
            SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expiresAt,
            IssuedAt = issuedAt,
            SigningCredentials = credentials,
            Audience = _jwtOptions.Audience,
            Issuer = _jwtOptions.Issuer
        };

        var handler = new JwtSecurityTokenHandler();
        var token = handler.CreateToken(tokenDescriptor);
        return (handler.WriteToken(token), expiresAt);
    }

    public Result<ClaimsPrincipal> ValidateToken(string token, bool validateLifetime = true)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = new SymmetricSecurityKey(_secret);

        try
        {
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _jwtOptions.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtOptions.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateLifetime = validateLifetime,
                ClockSkew = TimeSpan.Zero
            }, out var validatedToken);

            if (validatedToken is not JwtSecurityToken jwt 
                || !jwt.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return Result.Fail(Error.InvalidFormat("Invalid token"));
            }

            return Result.Ok(principal);
        }
        catch (Exception ex)
        {
            return Result.Fail(Error.InternalServerError($"Token validation failed: {ex.Message}"));
        }
    }
}

