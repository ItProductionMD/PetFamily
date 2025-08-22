using PetFamily.SharedKernel.Results;

namespace Authorization.Public.Contracts;

public interface IRoleContract
{
    Task<UnitResult> AssignRole(Guid userId,string roleCode, CancellationToken ct);
}
