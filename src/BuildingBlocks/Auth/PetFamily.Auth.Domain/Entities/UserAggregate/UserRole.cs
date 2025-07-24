using PetFamily.Auth.Domain.Entities.RoleAggregate;
using PetFamily.Auth.Domain.ValueObjects;
using PetFamily.SharedKernel.Abstractions;
using PetFamily.SharedKernel.ValueObjects.Ids;

namespace PetFamily.Auth.Domain.Entities.UserAggregate;

public class UserRole : SoftDeletable
{
    public UserId UserId { get; private set; }
    public RoleId RoleId { get; private set; }

    private UserRole() { }

    public UserRole(UserId userId, RoleId roleId)
    {
        UserId = userId;
        RoleId = roleId;
    }
}
