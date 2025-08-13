using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Org.BouncyCastle.Asn1.Ocsp;
using PetFamily.Auth.Application.Dtos;
using PetFamily.Auth.Application.Options;
using PetFamily.SharedApplication.Exceptions;

namespace PetFamily.Auth.Presentation.RefreshToken;

public class RefreshTokenCookieService(
    IHttpContextAccessor httpContextAccessor,
    IOptions<RefreshTokenCookie> options) : IRefreshTokenService
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly RefreshTokenCookie _options = options.Value;

    public string GetRefreshToken()
    {
        var refreshToken = _httpContextAccessor.HttpContext?.Request.Cookies["refreshToken"];
        if (string.IsNullOrWhiteSpace(refreshToken))
            throw new UserNotAuthenticatedException();

        return refreshToken;
    }

    public void SetRefreshToken(TokenResult tokenResult)
    {
        var response = _httpContextAccessor.HttpContext?.Response;
        if (response == null)
            throw new InvalidOperationException("HTTP Response is not available");

        var refreshToken = tokenResult.RefreshToken;
        var refreshTokenExpiresAt = tokenResult.RefreshTokenExpiresAt;

        var sameSiteMode = _options.SameSiteMode.ToLower() switch
        {
            "none" => SameSiteMode.None,
            "unspecified" => SameSiteMode.Unspecified,
            "strict" => SameSiteMode.Strict,
            "lax" => SameSiteMode.Lax,
            _ => throw new NotImplementedException("SameSiteMode from JWT options is incorrect!")

        };

        response.Cookies.Append(_options.CookieName, refreshToken, new CookieOptions
        {
            HttpOnly = _options.HttpOnly,
            Secure = _options.Secure,
            SameSite = SameSiteMode.None,
            Path = _options.CookiePath,
            Expires = refreshTokenExpiresAt
        });
    }
}

