using Microsoft.AspNetCore.Mvc;
using Account.Application.UserManagement.Commands.ConfirmEmail;
using Account.Application.UserManagement.Commands.LoginUserByEmail;
using Account.Application.UserManagement.Commands.RegisterByEmail;
using Account.Application.UserManagement.Queries.GetUserAccountInfo;
using Account.Presentation.Requests;
using PetFamily.Framework;
using PetFamily.Framework.Extensions;
using PetFamily.Framework.HTTPContext.Cookie;

namespace Account.Presentation.Controllers;

//TODO ADD PERMISSIONS
[ApiController]
[Route("api/users")]
public class UserController(ICookieService cookieService) : ControllerBase
{

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
        var command = request.ToCommand();

        var tokenResult = await handler.Handle(command, ct);
        if (tokenResult.IsSuccess)
            cookieService.SetRefreshToken(tokenResult.Data!);

        return tokenResult.ToActionResult();
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
}
