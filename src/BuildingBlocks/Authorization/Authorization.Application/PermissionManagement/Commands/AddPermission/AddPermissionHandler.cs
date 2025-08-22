using Authorization.Application.IRepositories.IAuthorizationRepo;
using Authorization.Domain.Entities;
using Microsoft.Extensions.Logging;
using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.SharedKernel.Results;

namespace Authorization.Application.PermissionManagement.Commands.AddPermission;

public class AddPermissionHandler(
    IPermissionWriteRepo permissionWriteRepo,
    ILogger<AddPermissionHandler> logger) : ICommandHandler<Guid, AddPermissionCommand>
{

    public async Task<Result<Guid>> Handle(AddPermissionCommand cmd, CancellationToken ct)
    {
        //TODO Validate command

        var permission = Permission.Create(cmd.Code).Data!;

        await permissionWriteRepo.AddPermissionAsync(permission, ct);

        logger.LogInformation("Add new permission successful! Id:{Id}", permission.Id);

        return Result.Ok(permission.Id);
    }
}
