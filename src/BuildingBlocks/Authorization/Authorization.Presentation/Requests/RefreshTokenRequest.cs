using Authorization.Application.RefreshTokenSessionManagement.Commands.RefreshTokens;
using System.Security.Claims;

namespace Authorization.Presentation.Requests;

public class RefreshTokensRequest()
{
    public string FingerPrint { get; set; }
    public string AccessToken { get; set; }

    public RefreshTokenCommand ToCommand(string refreshToken)
    {
        return new RefreshTokenCommand(AccessToken, refreshToken, FingerPrint);
    }
}
