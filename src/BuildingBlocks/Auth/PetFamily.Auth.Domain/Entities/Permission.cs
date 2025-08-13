using PetFamily.Auth.Domain.ValueObjects;
using PetFamily.SharedKernel.Abstractions;
using PetFamily.SharedKernel.Results;


namespace PetFamily.Auth.Domain.Entities;

public class Permission : IEntity<PermissionId>
{
    public PermissionId Id { get; private set; }
    public string Code { get; set; }
    public bool IsEnabled { get; set; } = true;

    private Permission() { }

    private Permission(PermissionId id, string code)
    {
        Id = id;
        Code = code;
    }

    public static Result<Permission> Create(string code)
    {
        //Validate name and displayName

        var permission = new Permission(PermissionId.New(), code);

        return Result.Ok(permission);
    }

    public void Disable() => IsEnabled = false;
    public void Enable() => IsEnabled = true;
}
