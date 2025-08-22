using PetFamily.SharedKernel.Results;

namespace Authorization.Domain.Entities.RoleAggregate;

public class RolePermission
{
    public Guid RoleId { get; private set; }
    public Guid PermissionId { get; private set; }

    private RolePermission() { } //EfCore need this

    private RolePermission(Guid roleId, Guid permissionId)
    {
        RoleId = roleId;
        PermissionId = permissionId;
    }

    public static Result<RolePermission> Create(
        Guid roleId,
        Guid permissionId)
    {
        return Result.Ok(new RolePermission(roleId, permissionId));
    }
}
