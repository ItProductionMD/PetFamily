using PetFamily.SharedKernel.Results;

namespace PetFamily.Framework.HTTPContext.Interfaces;

public interface IUserContext
{
    bool HasPermission(string permission);
    string Email { get; }
    string Phone { get; }

    Guid GetUserId();
}
