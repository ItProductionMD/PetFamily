using Microsoft.AspNetCore.Mvc;
using PetFamily.Auth.Application.UserManagement.Commands.ChangeRoles;
using PetFamily.Auth.Application.UserManagement.Commands.ConfirmEmail;
using PetFamily.Auth.Application.UserManagement.Commands.LoginUserByEmail;
using PetFamily.Auth.Application.UserManagement.Commands.RegisterByEmail;
using PetFamily.Auth.Presentation.Requests;
using PetFamily.Framework;
using PetFamily.Framework.Extensions;

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
            request.SocialNetworks
            );

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
        [FromServices] LoginByEmailCommandHandler handler,
        [FromBody] LoginByEmailRequest request,
        CancellationToken ct = default)
    {
        var cmd = new LoginByEmailCommand(
            request.Email,
            request.Password
            );

        var result = await handler.Handle(cmd, ct);
        return result.IsSuccess
            ? result.ToEnvelope()
            : result.ToErrorActionResult();
    }
}
