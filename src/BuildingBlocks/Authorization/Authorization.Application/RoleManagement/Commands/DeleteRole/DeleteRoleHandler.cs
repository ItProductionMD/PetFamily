using Authorization.Application.IRepositories.IAuthorizationRepo;
using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.SharedKernel.Results;

namespace Authorization.Application.RoleManagement.Commands.DeleteRole;

public class DeleteRoleCommandHandler(IRoleWriteRepo roleWriteRepository) 
    : ICommandHandler<DeleteRoleCommand>
{
    public async Task<UnitResult> Handle(DeleteRoleCommand cmd, CancellationToken ct)
    {
        //TODO validate command

        var deleteResult = await roleWriteRepository.DeleteRole(cmd.RoleId, ct);

        return deleteResult;
    }
}

