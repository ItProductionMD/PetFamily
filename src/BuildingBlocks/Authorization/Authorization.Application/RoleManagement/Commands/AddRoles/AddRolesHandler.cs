using Authorization.Application.IRepositories.IAuthorizationRepo;
using Authorization.Domain.Entities.RoleAggregate;
using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;

namespace Authorization.Application.RoleManagement.Commands.AddRoles;

public class AddRolesHandler(
    IRoleReadRepo roleReadRepo,
    IRoleWriteRepo roleWriteRepo,
    IPermissionReadRepo permissionReadRepo) : ICommandHandler<AddRolesResponse, AddRolesCommand>
{
    public async Task<Result<AddRolesResponse>> Handle(AddRolesCommand cmd, CancellationToken ct = default)
    {
        //validate command

        var allRoles = await roleReadRepo.GetRoles(ct);

        var existingPermissions = await permissionReadRepo.GetPermissionsAsync(ct);
        var existingPermissionsIds = existingPermissions.Select(p => p.PermissionId).ToList();


        var rolesWithExistingData = new List<NotAddedRole>();

        var rolesToAdd = new List<Role>();

        foreach (var role in cmd.roleDtos)
        {
            var existingRole = allRoles.FirstOrDefault(r => r.RoleCode == role.RoleCode);
            if (existingRole != null)
            {
                var errorRole = new NotAddedRole(
                    role.RoleName,
                    role.RoleCode,
                    "Role with such data already exists");

                rolesWithExistingData.Add(errorRole);
            }
            if (role.PermissionsIds.All(existingPermissionsIds.Contains) == false)
            {
                return Result.Fail(Error.NotFound($"Permission Id for Role:{role.RoleCode}"));
            }
            else
            {
                var roleToAdd = Role.Create(
                    Guid.Empty,
                    role.RoleCode,
                    role.PermissionsIds).Data!;

                rolesToAdd.Add(roleToAdd);
            }
        }

        await roleWriteRepo.AddRoles(rolesToAdd, ct);

        await roleWriteRepo.SaveChangesAsync(ct);

        var response = new AddRolesResponse
        {
            AddedRoles = rolesToAdd,
            NotAddedRoles = rolesWithExistingData
        };

        return Result.Ok(response);
    }
}

public class AddRolesResponse
{
    public List<Role> AddedRoles { get; set; } = new();
    public List<NotAddedRole> NotAddedRoles { get; set; } = new();
}
public record NotAddedRole(string name, string Code, string errorMessage);
