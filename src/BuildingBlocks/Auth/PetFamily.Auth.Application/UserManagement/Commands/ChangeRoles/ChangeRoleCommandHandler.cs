using Microsoft.Extensions.Logging;
using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.Auth.Application.IRepositories;
using PetFamily.Auth.Domain.ValueObjects;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.ValueObjects.Ids;

namespace PetFamily.Auth.Application.UserManagement.Commands.ChangeRoles;

public class ChangeRoleCommandHandler(
    IUserWriteRepository userWriteRepo,
    ILogger<ChangeRoleCommandHandler> logger,
    IRoleReadRepository roleReadRepo) : ICommandHandler<ChangeRoleCommand>
{
    private readonly ILogger<ChangeRoleCommandHandler> _logger = logger;
    private readonly IUserWriteRepository _userWriteRepo = userWriteRepo;
    private readonly IRoleReadRepository _roleReadRepo = roleReadRepo;
    public async Task<UnitResult> Handle(ChangeRoleCommand cmd, CancellationToken ct)
    {
        ChangeRoleValidator.Validate(cmd);

        var userId = UserId.Create(cmd.UserId).Data!;
        var roleId = RoleId.Create(cmd.RoleId).Data!;

        var getUser = await _userWriteRepo.GetByIdAsync(userId, ct);
        if (getUser.IsFailure)
            return UnitResult.Fail(getUser.Error);

        var user = getUser.Data!;

        var roleExistResult = await _roleReadRepo.VerifyRolesExist([cmd.RoleId], ct);
        if (roleExistResult.IsFailure)
            return UnitResult.Fail(roleExistResult.Error);

        user.ChangeRole(roleId);

        await _userWriteRepo.SaveChangesAsync(ct);

        return UnitResult.Ok();
    }
}
