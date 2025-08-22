using PetFamily.SharedApplication.Abstractions;

namespace PetFamily.SharedInfrastructure.JWTProvider;

public class JwtOptions : IApplicationOptions
{
    public const string SECTION_NAME = "JwtOptions";
    public string SecretKey { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public int AccessTokenLifetimeMinutes { get; set; }
    public int RefreshTokenLifetimeDays { get; set; }
    public int EmailTokenLifeTimeDays { get; set; }

    public static string GetSectionName() => SECTION_NAME;
    
}
