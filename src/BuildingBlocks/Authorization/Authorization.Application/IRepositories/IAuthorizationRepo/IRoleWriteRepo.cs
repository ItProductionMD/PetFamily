using Authorization.Domain.Entities.RoleAggregate;
using PetFamily.SharedKernel.Results;

namespace Authorization.Application.IRepositories.IAuthorizationRepo;

public interface IRoleWriteRepo
{
    Task<Result<Guid>> AddAndSaveAsync(Role role, CancellationToken ct = default);
    Task<Result<Role>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<Guid>> AddRole(Role role, CancellationToken ct = default);
    Task<UnitResult> DeleteRole(Guid roleId, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
    Task AddRoles(IEnumerable<Role> roles, CancellationToken ct = default);
}
