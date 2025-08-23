using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Security;
using Account.Application.Dtos;
using Account.Application.IServices.Email;
using PetFamily.SharedApplication.IJWTProvider;
using PetFamily.SharedInfrastructure.JWTProvider;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.ValueObjects.Ids;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Account.Infrastructure.Services.EmailService;

public class EmailConfirmationTokenProvider : IEmailConfirmationTokenProvider
{
    private readonly JwtOptions _jwtOptions;
    private readonly IJwtProvider _jwtProvider;
    private readonly byte[] _secret;
    private const string REASON_CLAIM = "reason";
    private const string EMAIL_CONFIRMATION_REASON = "email_confirmation";

    public EmailConfirmationTokenProvider(IOptions<JwtOptions> jwtOptions, IJwtProvider jwtProvider)
    {
        _jwtProvider = jwtProvider;
        _jwtOptions = jwtOptions.Value;
        _secret = Encoding.UTF8.GetBytes(_jwtOptions.SecretKey);
    }

    public string CreateEmailConfirmationToken(Guid userId)
    {
        var expiresAt = DateTime.UtcNow.AddDays(_jwtOptions.EmailTokenLifeTimeDays);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(REASON_CLAIM, EMAIL_CONFIRMATION_REASON)
        };

        var token = _jwtProvider.GenerateJwtTokenData(claims, expiresAt);

        return token.Token;
    }

    public Result<Guid> GetUserIdFromEmailConfirmationToken(string emailConfirmationToken)
    {
        var userIdResult = _jwtProvider.GetClaimValueByName(emailConfirmationToken, ClaimTypes.NameIdentifier);
        if (userIdResult.IsFailure)
        {
            return Result.Fail(Error.Authorization("Invalid email confirmation token."));
        }

        var userId = Guid.TryParse(userIdResult.Data, out var userIdGuid);
        if (userId == false || userIdGuid == Guid.Empty)
        {
            return Result.Fail(Error.Authorization("Invalid user ID in email confirmation token."));
        }

        return Result.Ok(userIdGuid);
    }

    public UnitResult ValidateEmailConfirmationToken(string emailConfirmationToken)
    {
        var validateResult = _jwtProvider.ValidateToken(emailConfirmationToken, true);
        if (validateResult.IsFailure)
        {
            return Result.Fail(validateResult.Error);
        }
        var claims = validateResult.Data!;
        var reasonClaim = claims.FindFirst(REASON_CLAIM);
        if (reasonClaim == null || reasonClaim.Value != EMAIL_CONFIRMATION_REASON)
        {
            return Result.Fail(Error.Authorization("Invalid reason claim in email confirmation token."));
        }
        return UnitResult.Ok();
    }
}

