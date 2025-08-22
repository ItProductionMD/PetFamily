using Authorization.Application.Dtos;
using Authorization.Application.IRepositories.IAuthorizationRepo;
using Authorization.Domain.Entities;
using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.SharedKernel.Results;

namespace Authorization.Application.PermissionManagement.Commands.AddPermissions;

public class AddPermissionsCommandHandler(
    IPermissionWriteRepo permissionWriteRepo,
    IPermissionReadRepo permissionReadRepo) 
    : ICommandHandler<AddPermissionsResponse, AddPermissionsCommand>
{

    public async Task<Result<AddPermissionsResponse>> Handle(
        AddPermissionsCommand cmd, 
        CancellationToken ct)
    {
        cmd.Validate();

        var permissionsToAdd = cmd.newPermissionCodes;

        var alreadyExistingPermissions = new List<PermissionDtoWithError>();

        // Check if permissions already exist in the database
        var existingPermissions = await permissionReadRepo.GetPermissionsAsync(ct);

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

        await permissionWriteRepo.AddPermissions(permissions, ct);
        await permissionWriteRepo.SaveAsync(ct);

        var permissionDtos = permissions
            .Select(p => new PermissionDto(p.Id, p.Code, p.IsEnabled))
            .ToList();

        var response = new AddPermissionsResponse
        {
            AddedPermissions = permissionDtos,
            PermissionDtoWithErrors = alreadyExistingPermissions
        };

        return Result.Ok(response);
    }
}
public class AddPermissionsResponse
{
    public List<PermissionDtoWithError> PermissionDtoWithErrors { get; set; } = [];
    public List<PermissionDto> AddedPermissions { get; set; } = [];
}
public record PermissionDtoWithError(string Code, string Error);
