using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PetFamily.Auth.Application.Dtos;
using PetFamily.Auth.Application.Options;
using PetFamily.Auth.Application.UserManagement.Commands.ChangeRoles;
using PetFamily.Auth.Application.UserManagement.Commands.ConfirmEmail;
using PetFamily.Auth.Application.UserManagement.Commands.LoginUserByEmail;
using PetFamily.Auth.Application.UserManagement.Commands.RefreshToken;
using PetFamily.Auth.Application.UserManagement.Commands.RegisterByEmail;
using PetFamily.Auth.Presentation.Cookies;
using PetFamily.Auth.Presentation.Requests;
using PetFamily.Framework;
using PetFamily.Framework.Extensions;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;

namespace PetFamily.Auth.Presentation.Controllers;

[ApiController]
[Route("api/users")]
public class UserController : Controller
{
    [HttpPost("register_by_email")]
    public async Task<ActionResult<Envelope>> RegisterByEmail(
        [FromServices] RegisterByEmailCommandHandler handler,
        [FromBody] RegistrationByEmailRequest request,
        CancellationToken ct = default)
    {
        var cmd = new RegisterByEmailCommand(
            request.Email,
            request.Login,
            request.Password,
            request.PhoneRegionCode,
            request.PhoneNumber,
            request.SocialNetworks);

        var result = await handler.Handle(cmd, ct);
        return result.IsSuccess
            ? result.ToEnvelope()
            : result.ToErrorActionResult();
    }

    [HttpGet("confirm_email/{token}")]
    public async Task<ActionResult<Envelope>> ConfirmEmail(
        [FromServices] ConfirmEmailCommandHandler handler,
        [FromRoute] string token,
        CancellationToken ct)
    {
        var cmd = new ConfirmEmailCommand(token);

        var result = await handler.Handle(cmd, ct);
        return result.IsSuccess
            ? result.ToEnvelope()
            : result.ToErrorActionResult();
    }

    [HttpPatch("{id}/change_roles")]
    public async Task<ActionResult<Envelope>> ChangeRoles(
        [FromServices] ChangeRolesCommandHandler handler,
        [FromRoute] Guid id,
        [FromBody] IEnumerable<Guid> roleIds,
        CancellationToken ct)
    {

        var cmd = new ChangeRolesCommand(id, roleIds);

        var result = await handler.Handle(cmd, ct);

        return result.IsSuccess
            ? result.ToEnvelope()
            : result.ToErrorActionResult();
    }

    [HttpPost("login")]
    public async Task<ActionResult<Envelope>> Login(
        [FromServices] IOptions<RefreshTokenCookie> cookieOptions,
        [FromServices] LoginByEmailCommandHandler handler,
        [FromBody] LoginByEmailRequest request,
        CancellationToken ct = default)
    {
        var tokenCookieOptions = cookieOptions.Value;

        var refreshToken = Request.Cookies["refreshToken"];

        var cmd = new LoginByEmailCommand(
            request.Email,
            request.Password,
            request.FingerPrint
            );

        var result = await handler.Handle(cmd, ct);
        if (result.IsFailure)
            return result.ToErrorActionResult();

        HTTPResponseCookiesSetter.SetRefreshTokenCookie(
            Response,
            result.Data!.RefreshToken,
            result.Data!.RefreshTokenExpiresAt,
            tokenCookieOptions);

        return Result.Ok(result.Data!.AccessToken).ToEnvelope();
    }

    [HttpPost("tokens")]
    public async Task<ActionResult<Envelope>> RefreshTokens(
        [FromServices] IOptions<RefreshTokenCookie> cookieOptions,
        [FromServices] RefreshTokensHandler handler,
        [FromBody] RefreshTokensRequest request,
        CancellationToken ct = default)
    {
        var tokenCookieOptions = cookieOptions.Value;

        var refreshToken = Request.Cookies["refreshToken"];

        if (string.IsNullOrWhiteSpace(refreshToken))
            return Result.Fail(Error.Authorization("Refresh Token doesn't exist"))
                .ToErrorActionResult();

        var command = new RefreshTokenCommand(request.AccessToken, refreshToken, request.FingerPrint);

        var result = await handler.Handle(command, ct);

        if (result.IsFailure)
            return result.ToErrorActionResult();

        HTTPResponseCookiesSetter.SetRefreshTokenCookie(
            Response,
            result.Data!.RefreshToken,
            result.Data!.RefreshTokenExpiresAt,
            tokenCookieOptions);

        return Result.Ok(result.Data!.AccessToken).ToEnvelope();
    }
}
