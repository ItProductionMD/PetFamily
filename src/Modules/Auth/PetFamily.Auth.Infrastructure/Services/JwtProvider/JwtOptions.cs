namespace PetFamily.Auth.Infrastructure.Services.JwtProvider;

public class JwtOptions
{
    public const string SECTION_NAME = "JwtOptions";
    public string SecretKey { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public int AccessTokenLifetimeMinutes { get; set; }
    public int RefreshTokenLifetimeDays { get; set; }
}

