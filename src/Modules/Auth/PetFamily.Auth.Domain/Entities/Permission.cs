using Org.BouncyCastle.Bcpg;
using PetFamily.Auth.Domain.ValueObjects;
using PetFamily.SharedKernel.Abstractions;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.ValueObjects;


namespace PetFamily.Auth.Domain.Entities;

public class Permission : Entity<PermissionId>
{
    public string Code { get; set; }
    public bool IsEnabled { get; set; } = true;

    private Permission(PermissionId id) : base(id) { }

    private Permission(PermissionId id, string code) : base(id)
    {
        Code = code;
    }

    public static Result<Permission> Create(string code) 
    {
        //Validate name and displayName

        var permission = new Permission (PermissionId.New(), code);

        return Result.Ok(permission);
    }

    public void Disable() => IsEnabled = false;
    public void Enable() => IsEnabled = true;
}
