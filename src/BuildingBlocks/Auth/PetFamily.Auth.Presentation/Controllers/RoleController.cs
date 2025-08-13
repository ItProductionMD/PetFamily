using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetFamily.Auth.Application.RoleManagement.Commands.UpdateRole;
using PetFamily.Auth.Application.RoleManagement.Queries.GetRoles;
using PetFamily.Auth.Domain.ValueObjects;
using PetFamily.Framework;
using PetFamily.Framework.Extensions;
using static PetFamily.SharedKernel.Authorization.PermissionCodes.RoleManagement;

namespace PetFamily.Auth.Presentation.Controllers;

[ApiController]
[Route("api/roles")]
public class RoleController : Controller
{
    [Authorize(Policy = RoleView)]
    [HttpGet]
    public async Task<ActionResult<Envelope>> GetRoles(
        [FromServices] GetRolesQueryHandler handler,
        CancellationToken ct = default)
    {
        var result = await handler.Handle(new GetRolesQuery(), ct);

        return result.IsSuccess
            ? result.ToEnvelope()
            : result.ToErrorActionResult();
    }

    [Authorize(Policy = RoleEdit)]
    [HttpPost("{id}")]
    public async Task<ActionResult<Envelope>> ChangeRolePermissions(
       [FromServices] UpdateRoleCommandHandler handler,
       [FromRoute] Guid id,
       [FromBody] IEnumerable<Guid> newPermissions,
       CancellationToken ct = default)
    {
        var roleId = RoleId.Create(id).Data!;
        var command = new UpdateRoleCommand(roleId, newPermissions);
        var result = await handler.Handle(command, ct);

        return result.IsSuccess
            ? result.ToEnvelope()
            : result.ToErrorActionResult();
    }

}
