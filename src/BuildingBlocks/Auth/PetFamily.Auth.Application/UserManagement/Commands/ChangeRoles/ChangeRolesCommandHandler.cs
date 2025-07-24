using Microsoft.Extensions.Logging;
using PetFamily.Application.Abstractions.CQRS;
using PetFamily.Auth.Application.IRepositories;
using PetFamily.Auth.Domain.ValueObjects;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.ValueObjects.Ids;

namespace PetFamily.Auth.Application.UserManagement.Commands.ChangeRoles;

public class ChangeRolesCommandHandler(
    IUserWriteRepository userWriteRepository,
    ILogger<ChangeRolesCommandHandler> logger,
    IRoleReadRepository roleReadRepository) : ICommandHandler<ChangeRolesCommand>
{
    private readonly ILogger<ChangeRolesCommandHandler> _logger = logger;
    private readonly IUserWriteRepository _userWriteRepository = userWriteRepository;
    private readonly IRoleReadRepository _roleReadRepository = roleReadRepository;
    public async Task<UnitResult> Handle(ChangeRolesCommand cmd, CancellationToken ct)
    {
        var userId = UserId.Create(cmd.UserId).Data!;

        var getUser = await _userWriteRepository.GetByIdAsync(userId, ct);
        if (getUser.IsFailure)
            return UnitResult.Fail(getUser.Error);

        var user = getUser.Data!;

        var roleIds = cmd.RoleIds.Select(roleId => RoleId.Create(roleId).Data!).ToList();

        var roleExistResult = await _roleReadRepository.VerifyRolesExist(cmd.RoleIds, ct);
        if (roleExistResult.IsFailure)
            return UnitResult.Fail(roleExistResult.Error);


        user.UpdateRoles(roleIds);

        await _userWriteRepository.SaveChangesAsync(ct);

        return UnitResult.Ok();
    }
}
