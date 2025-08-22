using PetFamily.SharedApplication.Abstractions;

namespace PetFamily.Framework.HTTPContext.Cookie;

public class RefreshTokenCookieOptions : IApplicationOptions
{
    public const string SECTION_NAME = "RefreshTokenCookie";
    public string CookieName { get; set; }
    public string CookiePath { get; set; }
    public bool HttpOnly { get; set; }
    public bool Secure { get; set; }
    public string SameSiteMode { get; set; }//Unspecified, Strict, None, Lax

    public static string GetSectionName() => SECTION_NAME;
}
