using Authorization.Application.Dtos;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.ValueObjects.Ids;

namespace Authorization.Application.IRepositories.IAuthorizationRepo;

public interface IRoleReadRepo
{
    Task<Result<RoleDto>> GetByCodeAsync(string roleCode, CancellationToken ct = default);
    Task<Result<RoleDto>> GetRoleByUserIdAsync(Guid userId, CancellationToken ct);
    Task<List<RoleDto>> GetRoles(CancellationToken ct = default);
    Task<UnitResult> VerifyRolesExist(IEnumerable<Guid> roleIds, CancellationToken ct = default);
}
