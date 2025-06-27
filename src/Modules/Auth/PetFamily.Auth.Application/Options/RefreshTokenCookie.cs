namespace PetFamily.Auth.Application.Options;

public class RefreshTokenCookie
{
    public const string SECTION_NAME = "RefreshTokenCookie";
    public string CookieName { get; set; }
    public string CookiePath { get; set; }

    public bool HttpOnly { get; set; }
    public bool Secure { get; set; }
    public string SameSiteMode { get; set; }//Unspecified, Strict, None, Lax
}
