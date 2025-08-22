using PetFamily.SharedKernel.Abstractions;
using PetFamily.SharedKernel.Results;
using static PetFamily.SharedKernel.Validations.ValidationConstants;
using static PetFamily.SharedKernel.Validations.ValidationExtensions;

namespace Authorization.Domain.Entities;

public class Permission : IEntity<Guid>
{
    public Guid Id { get; private set; }
    public string Code { get; set; }
    public bool IsEnabled { get; set; } = true;

    private Permission() { }

    private Permission(Guid id, string code)
    {
        Id = id;
        Code = code;
    }

    public static Result<Permission> Create(string code)
    {
        //TODO Validate name and displayName

        var permission = new Permission(Guid.NewGuid(), code);

        return Result.Ok(permission);
    }
    public UnitResult Validate(string code)
    {
        return ValidateRequiredField(code, "permission code", MAX_LENGTH_SHORT_TEXT);
    }

    public void Disable() => IsEnabled = false;
    public void Enable() => IsEnabled = true;
}
