using Authorization.Domain.Entities;
using PetFamily.SharedKernel.Results;

namespace Authorization.Application.IRepositories.IAuthorizationRepo;

public interface IPermissionWriteRepo
{
    Task<Result<Guid>> AddPermissionAsync(Permission permission, CancellationToken ct = default);

    Task<UnitResult> SaveAsync(CancellationToken ct = default);

    Task AddPermissions(
        IEnumerable<Permission> permissions,
        CancellationToken ct = default);
}
