using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;

namespace Account.Domain.ValueObjects;

public record RolePermissionId
{
    public Guid Value { get; }

    private RolePermissionId(Guid value)
    {
        Value = value;
    }

    public static Result<RolePermissionId> Create(Guid id)
    {
        if (id == Guid.Empty)
            Result.Fail(Error.GuidIsEmpty("RolePermission Id"));

        return Result.Ok(new RolePermissionId(id));
    }

    public static RolePermissionId Empty() => new RolePermissionId(Guid.Empty);

    public static RolePermissionId New() => new RolePermissionId(Guid.NewGuid());
}

