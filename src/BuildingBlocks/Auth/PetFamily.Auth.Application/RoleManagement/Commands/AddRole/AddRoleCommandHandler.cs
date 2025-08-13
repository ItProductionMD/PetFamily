using Microsoft.Extensions.Logging;
using PetFamily.Auth.Application.IRepositories;
using PetFamily.Auth.Domain.Entities.RoleAggregate;
using PetFamily.Auth.Domain.ValueObjects;
using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.SharedKernel.Results;

namespace PetFamily.Auth.Application.RoleManagement.Commands.AddRole;

public class AddRoleCommandHandler(
    IPermissionReadRepository permissionReadRepository,
    IRoleWriteRepository roleWriteRepository,
    IRoleReadRepository roleReadRepository,
    ILogger<AddRoleCommandHandler> logger) : ICommandHandler<Guid, AddRoleCommand>
{
    private readonly IPermissionReadRepository _permissionReadRepository = permissionReadRepository;
    private readonly IRoleWriteRepository _writeRepository = roleWriteRepository;
    private readonly IRoleReadRepository _readRepository = roleReadRepository;
    private readonly ILogger<AddRoleCommandHandler> _logger = logger;


    public async Task<Result<Guid>> Handle(AddRoleCommand cmd, CancellationToken ct)
    {
        //TODO ValidateCommand

        var distinctPermissionGuids = cmd.PermissionIds.Distinct();

        var existPermissionsResult = await _permissionReadRepository
            .VerifyPermissionsExist(distinctPermissionGuids, ct);

        if (existPermissionsResult.IsFailure)
            return Result.Fail(existPermissionsResult.Error);

        var permissionsIds = distinctPermissionGuids.Select(guidId => PermissionId.Create(guidId).Data!);

        var role = Role.Create(RoleId.New(), cmd.RoleCode, permissionsIds).Data!;

        var result = await _writeRepository.AddRole(role, ct);

        await _writeRepository.SaveChangesAsync(ct);

        _logger.LogInformation("Add new role successful! Id={Id}", result.Data!);

        return result;
    }
}
