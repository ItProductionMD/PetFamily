using Microsoft.AspNetCore.Http;
using PetFamily.Auth.Application.Options;
using PetFamily.SharedKernel.Results;

namespace PetFamily.Auth.Presentation.Cookies;

public static class HTTPResponseCookiesSetter
{
    public static void SetRefreshTokenCookie(
        HttpResponse response,
        string refreshToken,
        DateTime refreshTokenExpiresAt,
        RefreshTokenCookie tokenCookieOptions)
    {

        var sameSiteMode = tokenCookieOptions.SameSiteMode.ToLower() switch
        {
            "none" => SameSiteMode.None,
            "unspecified" => SameSiteMode.Unspecified,
            "strict" => SameSiteMode.Strict,
            "lax" => SameSiteMode.Lax,
            _ => throw new NotImplementedException("SameSiteMode from JWT options is incorrect!")

        };

        response.Cookies.Append(tokenCookieOptions.CookieName, refreshToken, new CookieOptions
        {
            HttpOnly = tokenCookieOptions.HttpOnly,
            Secure = tokenCookieOptions.Secure,
            SameSite = SameSiteMode.None,
            Path = tokenCookieOptions.CookiePath,
            Expires = refreshTokenExpiresAt
        });
    }
}
