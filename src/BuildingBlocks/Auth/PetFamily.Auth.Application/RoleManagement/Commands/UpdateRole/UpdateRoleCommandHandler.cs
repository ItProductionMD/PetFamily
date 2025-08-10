using Microsoft.Extensions.Logging;
using PetFamily.Application.Abstractions.CQRS;
using PetFamily.Auth.Application.IRepositories;
using PetFamily.Auth.Domain.ValueObjects;
using PetFamily.SharedKernel.Results;

namespace PetFamily.Auth.Application.RoleManagement.Commands.UpdateRole;

public class UpdateRoleCommandHandler(
    IRoleWriteRepository roleWriteRepository,
    IRoleReadRepository roleReadRepository,
    IPermissionReadRepository permissionReadRepository,
    ILogger<UpdateRoleCommandHandler> logger) : ICommandHandler<UpdateRoleCommand>
{
    private readonly IRoleWriteRepository _writeRepository = roleWriteRepository;
    private readonly IRoleReadRepository _readRepository = roleReadRepository;
    private readonly IPermissionReadRepository _permissionReadRepository = permissionReadRepository;
    private readonly ILogger<UpdateRoleCommandHandler> _logger = logger;

    public async Task<UnitResult> Handle(UpdateRoleCommand cmd, CancellationToken ct)
    {
        //Validate command
        var getRole = await _writeRepository.GetByIdAsync(cmd.RoleId, ct);
        if (getRole.IsFailure)
            return UnitResult.Fail(getRole.Error);

        var role = getRole.Data!;

        var permissionsId = cmd.PermissionsId.Select(guidId => PermissionId.Create(guidId).Data!);

        var updateResult = role.Update(permissionsId);
        if (updateResult.IsFailure)
            return UnitResult.Fail(updateResult.Error);

        var addedPermissionsIds = updateResult.Data!.Select(p => p.Value).ToList();
        if (addedPermissionsIds.Count == 0)
        {
            await _writeRepository.SaveChangesAsync(ct);

            _logger.LogInformation("Update Role with id:{Id} successful", role.Id);

            return UnitResult.Ok();
        }

        var existResult = await _permissionReadRepository.VerifyPermissionsExist(addedPermissionsIds, ct);
        if (existResult.IsFailure)
            return UnitResult.Fail(existResult.Error);

        await _writeRepository.SaveChangesAsync(ct);

        _logger.LogInformation("Update Role with id:{Id} successful, added:{count} permissions",
            role.Id, addedPermissionsIds.Count);

        return UnitResult.Ok();
    }
}