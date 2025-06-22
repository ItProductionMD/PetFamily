using PetFamily.Application.Abstractions.CQRS;
using PetFamily.Auth.Application.Dtos;
using PetFamily.Auth.Application.IRepositories;
using PetFamily.SharedKernel.Results;

namespace PetFamily.Auth.Application.PermissionManagement.Queries.GetPermissions;

public class GetPermissionsQueryHandler(
    IPermissionReadRepository repository) : IQueryHandler<List<PermissionDto>, GetPermissionsQuery>
{
    private readonly IPermissionReadRepository _repository = repository;

    public async Task<Result<List<PermissionDto>>> Handle(GetPermissionsQuery query, CancellationToken ct)
    {
        var permissionDtos = await _repository.GetPermissionsAsync(ct);

        return Result.Ok(permissionDtos);
    }
}
