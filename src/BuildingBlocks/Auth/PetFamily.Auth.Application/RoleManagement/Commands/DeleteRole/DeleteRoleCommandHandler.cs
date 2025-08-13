using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.Auth.Application.IRepositories;
using PetFamily.Auth.Domain.ValueObjects;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;

namespace PetFamily.Auth.Application.RoleManagement.Commands.DeleteRole;

public class DeleteRoleCommandHandler(
    IRoleWriteRepository roleWriteRepository,
    IUserReadRepository userReadRepository) : ICommandHandler<DeleteRoleCommand>
{
    public async Task<UnitResult> Handle(DeleteRoleCommand cmd, CancellationToken ct)
    {
        //TODO validate command
        var getRoleId = RoleId.Create(cmd.RoleId);
        if (getRoleId.IsFailure)
            return UnitResult.Fail(getRoleId.Error);

        var roleId = getRoleId.Data!;

        var isAnyUser = await userReadRepository.AnyUserWithRoleAsync(roleId, ct);
        if (isAnyUser)
            return UnitResult.Fail(Error.Conflict($"Role {roleId.Value} cannot be deleted because it is assigned to users."));

        var deleteResult = await roleWriteRepository.DeleteRole(roleId, ct);

        return deleteResult;
    }
}

