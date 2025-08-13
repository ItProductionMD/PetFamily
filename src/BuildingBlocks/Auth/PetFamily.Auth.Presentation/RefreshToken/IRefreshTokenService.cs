using PetFamily.Auth.Application.Dtos;

namespace PetFamily.Auth.Presentation.RefreshToken;

public interface IRefreshTokenService
{
    string GetRefreshToken();
    void SetRefreshToken(TokenResult tokenResult);
}
