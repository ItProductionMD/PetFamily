using PetFamily.Auth.Domain.Entities;
using PetFamily.SharedKernel.Results;

namespace PetFamily.Auth.Application.IRepositories;

public interface IPermissionWriteRepository
{
    Task<Result<Guid>> AddPermissionAsync(Permission permission, CancellationToken ct = default);

    Task<UnitResult> SaveAsync(CancellationToken ct = default);

    Task AddPermissions(
        IEnumerable<Permission> permissions,
        CancellationToken ct = default);
}
