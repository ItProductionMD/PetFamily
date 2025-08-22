using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PetFamily.SharedApplication.IJWTProvider;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PetFamily.SharedInfrastructure.JWTProvider;

public class JwtProvider : IJwtProvider 
{
    private readonly JwtOptions _jwtOptions;
    private readonly byte[] _secret;

    public JwtProvider(IOptions<JwtOptions> jwtOptions)
    {
        _jwtOptions = jwtOptions.Value;
        _secret = Encoding.UTF8.GetBytes(_jwtOptions.SecretKey);
    }

    public Result<string> GetClaimValueByName(string jwtToken, string claimName)
    {
        var claimsResult = ValidateToken(jwtToken, false);
        if (claimsResult.IsFailure)
            return Result.Fail(claimsResult.Error);

        var claims = claimsResult.Data;
        var claim = claims?.FindFirst(claimName);
        if (claim != null && !string.IsNullOrWhiteSpace(claim.Value))
            return Result.Ok(claim.Value);

        return Result.Fail(Error.Authentication($"Access token get claim :{claimName} error!"));
    }

    public (string Token, DateTime ExpiresAt) GenerateJwtTokenData(
        IEnumerable<Claim> claims, 
        DateTime expiresAtUtcNow)
    {
        var issuedAt = DateTime.UtcNow;

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(_secret),
            SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expiresAtUtcNow,
            IssuedAt = issuedAt,
            SigningCredentials = credentials,
            Audience = _jwtOptions.Audience,
            Issuer = _jwtOptions.Issuer
        };

        var handler = new JwtSecurityTokenHandler();

        var token = handler.CreateToken(tokenDescriptor);

        return (handler.WriteToken(token), expiresAtUtcNow);
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
                || !jwt.Header.Alg.Equals(
                    SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase))
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
