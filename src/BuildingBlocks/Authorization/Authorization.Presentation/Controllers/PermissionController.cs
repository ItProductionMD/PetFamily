using Authorization.Application.PermissionManagement.Commands.AddPermissions;
using Authorization.Application.PermissionManagement.Queries.GetPermissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetFamily.Framework;
using PetFamily.Framework.Extensions;
using System.Runtime.InteropServices;
using static PetFamily.SharedKernel.Authorization.PermissionCodes.PermissionManagement;

namespace Authorization.Presentation.Controllers;

[Route("api/permissions")]
[ApiController]
public class PermissionController : ControllerBase
{
    /// <summary>
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>List of permissions</returns>
    [Authorize(Policy = PermissionView)]
    [HttpGet]
    public async Task<ActionResult<Envelope>> GetPermissionsAsync(
        [FromServices] GetPermissionsQueryHandler handler,
        CancellationToken ct = default)
    {
        var query = new GetPermissionsQuery();
        return(await handler.Handle(query, ct)).ToActionResult();
    }

    [Authorize(Policy = PermissionCreate)]
    [HttpPost]
    public async Task<ActionResult<Envelope>> AddPermissionAsync(
        [FromBody] List<string> newPermissionCodes,
        [FromServices] AddPermissionsCommandHandler handler,
        CancellationToken ct = default)
    {
        var command = new AddPermissionsCommand(newPermissionCodes);
        return (await handler.Handle(command, ct)).ToActionResult();
    }

}

public record PermissionDto(string Code);