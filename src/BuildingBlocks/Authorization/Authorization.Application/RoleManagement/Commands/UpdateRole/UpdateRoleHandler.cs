using Authorization.Application.IRepositories.IAuthorizationRepo;
using Microsoft.Extensions.Logging;
using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.SharedKernel.Results;

namespace Authorization.Application.RoleManagement.Commands.UpdateRole;

public class UpdateRoleCommandHandler(
    IRoleWriteRepo roleWriteRepo,
    IPermissionReadRepo permissionReadRepo,
    ILogger<UpdateRoleCommandHandler> logger) : ICommandHandler<UpdateRoleCommand>
{
    public async Task<UnitResult> Handle(UpdateRoleCommand cmd, CancellationToken ct)
    {
        //Validate command
        var getRole = await roleWriteRepo.GetByIdAsync(cmd.RoleId, ct);
        if (getRole.IsFailure)
            return UnitResult.Fail(getRole.Error);

        var role = getRole.Data!;

        var permissionsId = cmd.PermissionsId;

        var updateResult = role.Update(permissionsId);
        if (updateResult.IsFailure)
            return UnitResult.Fail(updateResult.Error);

        var addedPermissionsIds = updateResult.Data!;
        if (addedPermissionsIds.Count == 0)
        {
            await roleWriteRepo.SaveChangesAsync(ct);

            logger.LogInformation("Update Role with id:{Id} successful", role.Id);

            return UnitResult.Ok();
        }

        var existResult = await permissionReadRepo.VerifyPermissionsExist(addedPermissionsIds, ct);
        if (existResult.IsFailure)
            return UnitResult.Fail(existResult.Error);

        await roleWriteRepo.SaveChangesAsync(ct);

        logger.LogInformation("Update Role with id:{Id} successful, added:{count} permissions",
            role.Id, addedPermissionsIds.Count);

        return UnitResult.Ok();
    }
}
