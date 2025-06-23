using PetFamily.SharedKernel.Results;

namespace PetFamily.Auth.Public.IContracts;

public interface IUserContext
{
    Result<Guid> GetUserId();
    bool HasPermission(string permission);
    string Email { get; }
    string Phone { get; }
}
