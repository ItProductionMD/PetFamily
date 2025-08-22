using PetFamily.SharedKernel.Abstractions;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.Uniqness;

namespace Authorization.Domain.Entities.RoleAggregate;
public class Role : IEntity<Guid>
{
    public Guid Id { get; private set; }
    [Unique]
    public string Code { get; private set; }

    private readonly List<RolePermission> _rolePermissions = [];
    public IReadOnlyCollection<RolePermission> RolePermissions => _rolePermissions.AsReadOnly();

    private Role() { }//EFCore need this

    private Role(Guid id, string code, IEnumerable<RolePermission> permissions)
    {
        Id = id;
        Code = code;
        _rolePermissions = permissions.ToList();
    }

    public static Result<Role> Create(
        Guid roleId,
        string code,
        IEnumerable<Guid> PermissionsIds)
    {
        if (string.IsNullOrWhiteSpace(code))
            return Result.Fail(Error.StringIsNullOrEmpty("Role code cannot be empty."));

        var rolePermissions = PermissionsIds.Select(permissionId => RolePermission.Create(
            roleId,
            permissionId).Data!);

        var role = new Role(roleId, code, rolePermissions);

        return Result.Ok(role);
    }

    public List<Guid> UpdatePermissions(IEnumerable<Guid> newPermissionIds)
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


    public void AddPermission(Guid permissionId)
    {
        if (_rolePermissions.Any(rp => rp.PermissionId == permissionId))
            return;

        var newRp = RolePermission.Create(Id, permissionId);
        if (newRp.IsSuccess)
            _rolePermissions.Add(newRp.Data!);
    }

    public void RemovePermission(Guid permissionId)
    {
        _rolePermissions.ToList().RemoveAll(rp => rp.PermissionId == permissionId);
    }

    public bool CanBeAssignedToUser() => true;//TODO

    public Result<List<Guid>> Update(IEnumerable<Guid> permissionsId)
    {
        var addedPermissions = UpdatePermissions(permissionsId);
        return Result.Ok(addedPermissions);
    }
}


