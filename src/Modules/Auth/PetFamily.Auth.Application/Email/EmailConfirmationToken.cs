using PetFamily.Auth.Application.Constants;
using PetFamily.Auth.Domain.Entities.UserAggregate;
using System.Security.Claims;

namespace PetFamily.Auth.Application.Email;

public class EmailConfirmationToken
{
    public const string CLAIM_REASON = "email_confirmation";
    public const int EXPIRES_DAY = 5;
    public DateTime IssueDay { get; set; } = DateTime.UtcNow;
    public DateTime ExpiredAt => IssueDay + TimeSpan.FromDays(EXPIRES_DAY);

    public List<Claim> claims { get; set; } = [];

    public EmailConfirmationToken(User user)
    {
        claims = GenerateClaims(user);
    }

    private static List<Claim> GenerateClaims(User user)
    {
        return new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.Value.ToString()),
            new Claim(CustomClaimTypes.REASON, CLAIM_REASON),
        };
    }
}
