using Authorization.Application.Dtos;
using PetFamily.SharedKernel.Results;

namespace Authorization.Application.IRepositories.IAuthorizationRepo;

public interface IPermissionReadRepo
{
    Task<List<PermissionDto>> GetPermissionsAsync(CancellationToken ct = default);
    Task<UnitResult> VerifyPermissionsExist(IEnumerable<Guid> permissionsIds, CancellationToken ct = default);

}
