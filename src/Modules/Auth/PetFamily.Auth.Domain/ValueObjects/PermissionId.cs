using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;

namespace PetFamily.Auth.Domain.ValueObjects;

public record PermissionId
{
    public Guid Value { get; }

    private PermissionId(Guid value)
    {
        Value = value;
    }

    public static Result<PermissionId> Create(Guid id)
    {
        if (id == Guid.Empty)
            Result.Fail(Error.GuidIsEmpty("Permission Id"));

        return Result.Ok(new PermissionId(id));
    }

    public static PermissionId Empty() => new PermissionId(Guid.Empty);

    public static PermissionId New() => new PermissionId(Guid.NewGuid());
}

