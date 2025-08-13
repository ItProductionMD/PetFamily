using PetFamily.Auth.Domain.ValueObjects;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;

namespace PetFamily.Auth.Domain.Entities.RoleAggregate;

public class RolePermission
{
    public RoleId RoleId { get; private set; }
    public PermissionId PermissionId { get; private set; }

    private RolePermission() { } //EfCore need this

    private RolePermission(RoleId roleId, PermissionId permissionId)
    {
        RoleId = roleId;
        PermissionId = permissionId;
    }

    public static Result<RolePermission> Create(
        RoleId roleId,
        PermissionId permissionId)
    {
        if (roleId.Value == Guid.Empty)
            return Result.Fail(Error.GuidIsEmpty("RolePermission RoleId"));

        if (permissionId.Value == Guid.Empty)
            return Result.Fail(Error.GuidIsEmpty("RolePermission PermissionId"));

        return Result.Ok(new RolePermission(roleId, permissionId));
    }
}

