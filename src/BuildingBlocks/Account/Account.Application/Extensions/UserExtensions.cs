using Account.Domain.Entities.UserAggregate;
using System.Security.Claims;

namespace Account.Application.Extensions;

public static class UserExtensions
{
    public static List<Claim> CreateClaims(this User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Login),
        };

        if (!string.IsNullOrWhiteSpace(user.Phone.Number) && !string.IsNullOrWhiteSpace(user.Phone.RegionCode))
        {
            claims.Add(new Claim(ClaimTypes.MobilePhone, user.Phone.ToString()));
        }
        if (!string.IsNullOrWhiteSpace(user.Email))
        {
            claims.Add(new Claim(ClaimTypes.Email, user.Email));
        }
        return claims;
    }
}
