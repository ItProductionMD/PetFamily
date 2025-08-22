using Authorization.Public.Contracts;
using PetFamily.SharedKernel.Results;

namespace Authorization.Infrastructure.Contracts;

internal class RoleContract : IRoleContract
{
    public Task<UnitResult> AssignRole(Guid userId, string roleCode, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
