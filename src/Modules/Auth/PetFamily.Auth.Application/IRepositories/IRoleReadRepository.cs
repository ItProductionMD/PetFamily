using PetFamily.Auth.Application.Dtos;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.ValueObjects.Ids;

namespace PetFamily.Auth.Application.IRepositories;

public interface IRoleReadRepository
{
    Task<Result<RoleDto>> GetByCodeAsync(string roleCode, CancellationToken ct = default);
    Task<List<RoleDto>> GetRolesByUserId(UserId userId, CancellationToken ct);
    Task<List<RoleDto>> GetRoles(CancellationToken ct = default);
    Task<UnitResult> VerifyRolesExist(IEnumerable<Guid> roleIds, CancellationToken ct = default);
}

