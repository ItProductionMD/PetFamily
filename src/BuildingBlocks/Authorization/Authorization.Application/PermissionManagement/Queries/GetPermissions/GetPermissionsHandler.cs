using Authorization.Application.Dtos;
using Authorization.Application.IRepositories.IAuthorizationRepo;
using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.SharedKernel.Results;

namespace Authorization.Application.PermissionManagement.Queries.GetPermissions;

public class GetPermissionsQueryHandler(
    IPermissionReadRepo permissionReadRepo) : IQueryHandler<List<PermissionDto>, GetPermissionsQuery>
{

    public async Task<Result<List<PermissionDto>>> Handle(GetPermissionsQuery query, CancellationToken ct)
    {
        var permissionDtos = await permissionReadRepo.GetPermissionsAsync(ct);

        return Result.Ok(permissionDtos);
    }
}
