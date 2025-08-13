using PetFamily.Auth.Application.UserManagement.Commands.RefreshToken;

namespace PetFamily.Auth.Presentation.Requests;

public class RefreshTokensRequest()
{
    public string FingerPrint { get; set; }
    public string AccessToken { get; set; }

    public RefreshTokenCommand ToCommand(string refreshToken)
    {
        return new RefreshTokenCommand(AccessToken, refreshToken, FingerPrint);
    }
}