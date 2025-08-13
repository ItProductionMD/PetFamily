using PetFamily.Auth.Domain.ValueObjects;
using PetFamily.SharedKernel.Abstractions;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.Uniqness;

namespace PetFamily.Auth.Domain.Entities.RoleAggregate;


public class Role : IEntity<RoleId>
{
    public RoleId Id { get; private set; }
    [Unique]
    public string Code { get; private set; }

    private readonly List<RolePermission> _rolePermissions = [];
    public IReadOnlyCollection<RolePermission> RolePermissions => _rolePermissions.AsReadOnly();

    private Role() { }//EFCore need this

    private Role(RoleId id, string code, IEnumerable<RolePermission> permissions)
    {
        Id = id;
        Code = code;
        _rolePermissions = permissions.ToList();
    }

    public static Result<Role> Create(
        RoleId roleId,
        string code,
        IEnumerable<PermissionId> PermissionsIds)
    {
        if (string.IsNullOrWhiteSpace(code))
            return Result.Fail(Error.StringIsNullOrEmpty("Role code cannot be empty."));

        var rolePermissions = PermissionsIds.Select(permissionId => RolePermission.Create(
            roleId,
            permissionId).Data!);

        var role = new Role(roleId, code, rolePermissions);

        return Result.Ok(role);
    }

    public List<PermissionId> UpdatePermissions(IEnumerable<PermissionId> newPermissionIds)
    {
        var distinctPermissionIds = newPermissionIds.Distinct().ToList();

        var toAdd = distinctPermissionIds
            .Except(_rolePermissions.Select(rp => rp.PermissionId))
            .ToList();

        var toRemove = _rolePermissions
            .Where(rp => !distinctPermissionIds.Contains(rp.PermissionId))
            .Select(rp => rp.PermissionId)
            .ToList();

        foreach (var permissionId in toRemove)
            RemovePermission(permissionId);

        foreach (var permissionId in toAdd)
            AddPermission(permissionId);

        return toAdd;
    }


    public void AddPermission(PermissionId permissionId)
    {
        if (_rolePermissions.Any(rp => rp.PermissionId.Value == permissionId.Value))
            return;

        var newRp = RolePermission.Create(Id, permissionId);
        if (newRp.IsSuccess)
            _rolePermissions.Add(newRp.Data!);
    }

    public void RemovePermission(PermissionId permissionId)
    {
        _rolePermissions.ToList().RemoveAll(rp => rp.PermissionId == permissionId);
    }

    public bool CanBeAssignedToUser() => true;//TODO

    public Result<List<PermissionId>> Update(IEnumerable<PermissionId> permissionsId)
    {
        var addedPermissions = UpdatePermissions(permissionsId);
        return Result.Ok(addedPermissions);
    }
}

