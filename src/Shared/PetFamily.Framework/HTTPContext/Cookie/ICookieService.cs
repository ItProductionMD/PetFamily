using Authorization.Public.Dtos;

namespace PetFamily.Framework.HTTPContext.Cookie;

public interface ICookieService
{
    public string GetRefreshToken();
    void SetRefreshToken(AuthorizationTokens tokenResult);
}
