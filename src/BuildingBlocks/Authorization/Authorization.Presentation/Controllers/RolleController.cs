using Authorization.Application.RoleManagement.Commands.UpdateRole;
using Authorization.Application.RoleManagement.Queries.GetRoles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetFamily.Framework;
using PetFamily.Framework.Extensions;
using static PetFamily.SharedKernel.Authorization.PermissionCodes.RoleManagement;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace Authorization.Presentation.Controllers;

[ApiController]
[Route("api/roles")]
public class RoleController : ControllerBase
{
    [Authorize(Policy = RoleView)]
    [HttpGet]
    public async Task<ActionResult<Envelope>> GetRoles(
        [FromServices] GetRolesQueryHandler handler,
        CancellationToken ct = default)
    {
        return (await handler.Handle(new GetRolesQuery(), ct)).ToActionResult();
    }

    [Authorize(Policy = RoleEdit)]
    [HttpPost("{id}")]
    public async Task<ActionResult<Envelope>> ChangeRolePermissions(
       [FromServices] UpdateRoleCommandHandler handler,
       [FromRoute] Guid id,
       [FromBody] IEnumerable<Guid> newPermissions,
       CancellationToken ct = default)
    {
        var command = new UpdateRoleCommand(id, newPermissions);
        return (await handler.Handle(command, ct)).ToActionResult();
    }
}
