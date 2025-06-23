using PetFamily.SharedKernel.Results;

namespace PetFamily.SharedApplication.IUserContext;

public interface IUserContext
{
    Result<Guid> GetUserId();
    bool HasPermission(string permission);
    string Email { get; }
    string Phone { get; }
}

