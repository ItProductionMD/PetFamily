using Microsoft.AspNetCore.Http;
using PetFamily.Auth.Application.Dtos;
using PetFamily.Auth.Application.Options;
using PetFamily.SharedKernel.Results;

namespace PetFamily.Auth.Presentation.Cookies;

public static class HTTPResponseCookiesSetter
{
    public static void SetRefreshTokenInCookies(
        HttpResponse response,
        TokenResult tokenResult,
        RefreshTokenCookie tokenCookieOptions)
    {

        var refreshToken = tokenResult.RefreshToken;
        var refreshTokenExpiresAt = tokenResult.RefreshTokenExpiresAt;

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
