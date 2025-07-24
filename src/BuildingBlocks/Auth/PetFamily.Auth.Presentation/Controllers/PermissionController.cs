using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Cms;
using PetFamily.Auth.Application.PermissionManagement.Commands.AddPermission;
using PetFamily.Auth.Application.PermissionManagement.Commands.AddPermissions;
using PetFamily.Auth.Application.PermissionManagement.Queries.GetPermissions;
using PetFamily.Auth.Domain.ValueObjects;
using PetFamily.Framework;
using PetFamily.Framework.Extensions;
using PetFamily.SharedKernel.ValueObjects.Ids;
using System.Runtime.InteropServices;
using static PetFamily.SharedKernel.Authorization.PermissionCodes.PermissionManagement;

namespace PetFamily.Auth.Presentation.Controllers;


[Route("api/permissions")]
[ApiController]
public class PermissionController : Controller
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

        var getPermissions = await handler.Handle(query,ct);
        
        return getPermissions.IsFailure
            ? getPermissions.ToErrorActionResult()
            : getPermissions.ToEnvelope();
    }

    [Authorize(Policy = PermissionCreate)]
    [HttpPost]
    public async Task<ActionResult<Envelope>> AddPermissionAsync(
        [FromBody] List<string> newPermissionCodes,
        [FromServices] AddPermissionsCommandHandler handler,
        CancellationToken ct = default)
    {
        var command = new AddPermissionsCommand(newPermissionCodes);

        var result = await handler.Handle(command, ct);

        return result.IsFailure
            ? result.ToErrorActionResult()
            : result.ToEnvelope();
    }

}

public record PermissionDto(string Code);

