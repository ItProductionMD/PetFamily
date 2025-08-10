using PetFamily.Auth.Domain.Entities.RoleAggregate;
using PetFamily.Auth.Domain.ValueObjects;
using PetFamily.SharedKernel.Results;

namespace PetFamily.Auth.Application.IRepositories;

public interface IRoleWriteRepository
{
    Task<Result<Role>> GetByIdAsync(RoleId id, CancellationToken ct = default);
    Task<Result<Guid>> AddRole(Role role, CancellationToken ct = default);
    Task<UnitResult> DeleteRole(RoleId roleId, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
    Task AddRoles(IEnumerable<Role> roles, CancellationToken ct = default);
}
