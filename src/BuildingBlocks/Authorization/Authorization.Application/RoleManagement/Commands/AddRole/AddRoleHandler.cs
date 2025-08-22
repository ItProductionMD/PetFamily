using Authorization.Application.IRepositories.IAuthorizationRepo;
using Authorization.Domain.Entities.RoleAggregate;
using Microsoft.Extensions.Logging;
using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.SharedKernel.Results;

namespace Authorization.Application.RoleManagement.Commands.AddRole;

public class AddRoleHandler(
    IPermissionReadRepo permissionReadRepo,
    IRoleWriteRepo roleWriteRepo,
    ILogger<AddRoleHandler> logger) : ICommandHandler<Guid, AddRoleCommand>
{
    public async Task<Result<Guid>> Handle(AddRoleCommand cmd, CancellationToken ct)
    {
        //TODO ValidateCommand

        var permissionsIds = cmd.PermissionIds.Distinct();

        var existPermissionsResult = await permissionReadRepo
            .VerifyPermissionsExist(permissionsIds, ct);

        if (existPermissionsResult.IsFailure)
            return Result.Fail(existPermissionsResult.Error);

        var role = Role.Create(Guid.NewGuid(), cmd.RoleCode, permissionsIds).Data!;

        var result = await roleWriteRepo.AddAndSaveAsync(role, ct);

        logger.LogInformation("Add new role successful! Id={Id}", result.Data!);

        return result;
    }
}
