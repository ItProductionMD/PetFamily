using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;

namespace PetFamily.Auth.Domain.ValueObjects;

public record RoleId
{
    public Guid Value { get; }

    private RoleId(Guid value)
    {
        Value = value;
    }

    public static Result<RoleId> Create(Guid id)
    {
        if (id == Guid.Empty)
            Result.Fail(Error.GuidIsEmpty("Role Id"));

        return Result.Ok(new RoleId(id));
    }

    public static RoleId Empty() => new RoleId(Guid.Empty);

    public static RoleId New() => new RoleId(Guid.NewGuid());
}
