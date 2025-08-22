using Authorization.Application.RefreshTokenSessionManagement.Commands.RefreshTokens;
using Authorization.Presentation.Requests;
using Microsoft.AspNetCore.Mvc;
using PetFamily.Framework;
using PetFamily.Framework.Extensions;
using PetFamily.Framework.HTTPContext.Cookie;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;


namespace Authorization.Presentation.Controllers;

public class TokenController(ICookieService cookieService) : ControllerBase
{
    /// <summary>
    /// Refreshes the access and refresh tokens using the refresh token stored in cookies.
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="request"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [HttpPost("tokens")]
    public async Task<ActionResult<Envelope>> RefreshTokens(
       [FromServices] RefreshTokensHandler handler,
       [FromBody] RefreshTokensRequest request,
       CancellationToken ct = default)
    {
        var refreshToken = cookieService.GetRefreshToken();
        if (string.IsNullOrEmpty(refreshToken))
            return Result.Fail(Error.Authentication("Refresh token is missing or invalid!"))
                .ToActionResult();

        var command = request.ToCommand(refreshToken);

        var tokenResult = await handler.Handle(command, ct);
        if (tokenResult.IsSuccess)
            cookieService.SetRefreshToken(tokenResult.Data!);

        return tokenResult.ToActionResult();
    }
}