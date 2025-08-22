using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;

namespace PetFamily.SharedKernel.ValueObjects.Ids;

public record struct UserId
{
    public Guid Value { get; }
    private UserId(Guid value)
    {
        Value = value;
    }
    public static Result<UserId> Create(Guid value)
    {
        if (value == Guid.Empty)
            return Result.Fail(Error.GuidIsEmpty("user id"));
        return Result.Ok(new UserId(value));
    }
    public static UserId NewGuid() => new(Guid.NewGuid());
    public static UserId Empty() => new(Guid.Empty);
}