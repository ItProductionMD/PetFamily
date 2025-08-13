using Microsoft.AspNetCore.Mvc;
using PetFamily.Auth.Application.Dtos;
using PetFamily.Auth.Application.UserManagement.Commands.ChangeRoles;
using PetFamily.Auth.Application.UserManagement.Commands.ConfirmEmail;
using PetFamily.Auth.Application.UserManagement.Commands.LoginUserByEmail;
using PetFamily.Auth.Application.UserManagement.Commands.RefreshToken;
using PetFamily.Auth.Application.UserManagement.Commands.RegisterByEmail;
using PetFamily.Auth.Application.UserManagement.Queries.GetUserAccountInfo;
using PetFamily.Auth.Presentation.RefreshToken;
using PetFamily.Auth.Presentation.Requests;
using PetFamily.Framework;
using PetFamily.Framework.Extensions;
using PetFamily.SharedKernel.Results;

namespace PetFamily.Auth.Presentation.Controllers;

//TODO ADD PERMISSIONS
[ApiController]
[Route("api/users")]
public class UserController(IRefreshTokenService refreshTokenService) : Controller
{
    private readonly IRefreshTokenService _refreshTokenService = refreshTokenService;

    /// <summary>
    /// Registers a new user by email.
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="request"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [HttpPost("register_by_email")]
    public async Task<ActionResult<Envelope>> RegisterByEmail(
        [FromServices] RegisterByEmailCommandHandler handler,
        [FromBody] RegistrationByEmailRequest request,
        CancellationToken ct = default)
    {
        var command = request.ToCommand();
        return (await handler.Handle(command, ct)).ToActionResult();
    }

    /// <summary>
    /// Confirms the user's email address using a confirmation token.
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="token"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [HttpGet("confirm_email/{token}")]
    public async Task<ActionResult<Envelope>> ConfirmEmail(
        [FromServices] ConfirmEmailHandler handler,
        [FromRoute] string token,
        CancellationToken ct)
    {
        var command = new ConfirmEmailCommand(token);
        return (await handler.Handle(command, ct)).ToActionResult();
    }

    /// <summary>
    /// Changes the role of a user.
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="userId"></param>
    /// <param name="roleId"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [HttpPatch("{id}/change_roles")]
    public async Task<ActionResult<Envelope>> ChangeRoles(
        [FromServices] ChangeRoleCommandHandler handler,
        [FromRoute] Guid userId,
        [FromBody] Guid roleId,
        CancellationToken ct)
    {
        var cmd = new ChangeRoleCommand(userId, roleId);
        return (await handler.Handle(cmd, ct)).ToActionResult();
    }

    /// <summary>
    /// Logs in a user by email and sets a refresh token in cookies.
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="request"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [HttpPost("login")]
    public async Task<ActionResult<Envelope>> Login(
        [FromServices] LoginByEmailCommandHandler handler,
        [FromBody] LoginByEmailRequest request,
        CancellationToken ct = default)
    {
        var refreshToken = _refreshTokenService.GetRefreshToken();

        var command = request.ToCommand();

        var tokenResult = await handler.Handle(command, ct);

        return HandleTokenResult(tokenResult);
    }

    /// <summary>
    /// Refreshes the access token using the refresh token from cookies.
    /// </summary>
    /// <param name="cookieOptions"></param>
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
        var refreshToken = _refreshTokenService.GetRefreshToken();

        var command = request.ToCommand(refreshToken);

        var tokenResult = await handler.Handle(command, ct);

        return HandleTokenResult(tokenResult);
    }

    /// <summary>
    /// Retrieves the account information of a user by their ID.
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="id"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [HttpGet("account_info/{id:Guid}")]
    public async Task<ActionResult<Envelope>> GetUserAccountInfo(
        [FromServices] GetUserAccountInfoHandler handler,
        [FromRoute] Guid id,
        CancellationToken ct)
    {
        var cmd = new GetUserAccountInfoCommand(id);
        return (await handler.Handle(cmd, ct)).ToActionResult();
    }

    private ActionResult<Envelope> HandleTokenResult(Result<TokenResult> tokenResult)
    {
        if (tokenResult.IsFailure)
            return tokenResult.ToErrorActionResult();

        _refreshTokenService.SetRefreshToken(tokenResult.Data!);

        return Result.Ok(tokenResult.Data!.AccessToken).ToActionResult();
    }
}
