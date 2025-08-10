using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.Auth.Application.IRepositories;
using PetFamily.Auth.Domain.Entities.RoleAggregate;
using PetFamily.Auth.Domain.ValueObjects;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;

namespace PetFamily.Auth.Application.RoleManagement.Commands.AddRoles;

public class AddRolesCommandHandler(
    IRoleReadRepository roleReadRepository,
    IRoleWriteRepository roleWriteRepository,
    IPermissionReadRepository permissionReadRepository) : ICommandHandler<AddRolesResponse, AddRolesCommand>
{
    private readonly IRoleReadRepository _roleReadRepository = roleReadRepository;
    private readonly IRoleWriteRepository _roleWriteRepository = roleWriteRepository;
    private readonly IPermissionReadRepository _permissionReadRepository = permissionReadRepository;

    public async Task<Result<AddRolesResponse>> Handle(AddRolesCommand cmd, CancellationToken ct = default)
    {
        //validate command

        var allRoles = await _roleReadRepository.GetRoles(ct);

        var existingPermissions = await _permissionReadRepository.GetPermissionsAsync(ct);
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
                    RoleId.Empty(),
                    role.RoleCode,
                    role.PermissionsIds.Select(id => PermissionId.Create(id).Data!)).Data!;

                rolesToAdd.Add(roleToAdd);
            }
        }

        await _roleWriteRepository.AddRoles(rolesToAdd, ct);

        await _roleWriteRepository.SaveChangesAsync(ct);

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
