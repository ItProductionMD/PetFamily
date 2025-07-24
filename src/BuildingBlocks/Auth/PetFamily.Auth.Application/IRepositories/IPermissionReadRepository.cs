using PetFamily.Auth.Application.Dtos;
using PetFamily.SharedKernel.Results;

namespace PetFamily.Auth.Application.IRepositories;

public interface IPermissionReadRepository
{
    Task<List<PermissionDto>> GetPermissionsAsync(CancellationToken ct = default);
    Task<UnitResult> VerifyPermissionsExist(IEnumerable<Guid> permissionsIds, CancellationToken ct = default);

}
