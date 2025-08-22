using PetFamily.SharedKernel.Results;

namespace Authorization.Public.Contracts;

public interface IAdminAuthorizationCreator
{
    Task<UnitResult> CreateAdminAuthorization(Guid userId, CancellationToken ct);
}
