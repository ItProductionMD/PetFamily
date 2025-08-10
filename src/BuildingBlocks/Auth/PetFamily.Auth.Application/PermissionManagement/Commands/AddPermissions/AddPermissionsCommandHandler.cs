using PetFamily.Application.Abstractions.CQRS;
using PetFamily.Auth.Application.Dtos;
using PetFamily.Auth.Application.IRepositories;
using PetFamily.Auth.Domain.Entities;
using PetFamily.SharedKernel.Results;

namespace PetFamily.Auth.Application.PermissionManagement.Commands.AddPermissions;

public class AddPermissionsCommandHandler(
    IPermissionWriteRepository permissionWriteRepository,
    IPermissionReadRepository permissionReadRepository) : ICommandHandler<AddPermissionsResponse, AddPermissionsCommand>
{
    private readonly IPermissionWriteRepository _permissionWriteRepository = permissionWriteRepository;
    private readonly IPermissionReadRepository _permissionReadRepository = permissionReadRepository;

    public async Task<Result<AddPermissionsResponse>> Handle(AddPermissionsCommand cmd, CancellationToken ct)
    {
        var validateCommand = AddPermissionsCommandValidator.Validate(cmd);
        if (validateCommand.IsFailure)
            return Result.Fail(validateCommand.Error);

        var permissionsToAdd = cmd.newPermissionCodes;

        var alreadyExistingPermissions = new List<PermissionDtoWithError>();

        // Check if permissions already exist in the database
        var existingPermissions = await _permissionReadRepository.GetPermissionsAsync(ct);

        foreach (var existingPermission in existingPermissions)
        {
            var permissionToAdd = permissionsToAdd.FirstOrDefault(
                newCode => newCode == existingPermission.PermissionCode);

            if (permissionToAdd != null)
            {
                permissionsToAdd.Remove(permissionToAdd);

                var errorPermission = new PermissionDtoWithError(
                    permissionToAdd,
                    $"Permission with souch data already exist(name:{existingPermission.PermissionCode}");

                alreadyExistingPermissions.Add(errorPermission);
            }
        }

        // Add permissions to repository
        var permissions = permissionsToAdd.Select(newCode => Permission.Create(newCode).Data!);

        await _permissionWriteRepository.AddPermissions(permissions, ct);
        await _permissionWriteRepository.SaveAsync(ct);

        var permissionDtos = permissions
            .Select(p => new PermissionDto(p.Id.Value, p.Code, p.IsEnabled))
            .ToList();

        var response = new AddPermissionsResponse
        {
            AddedPermissions = permissionDtos,
            PermissionDtoWithErrors = alreadyExistingPermissions
        };

        return Result.Ok(response);
    }
}
