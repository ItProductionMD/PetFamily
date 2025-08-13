using PetFamily.Auth.Application.Dtos;
using PetFamily.Auth.Application.IRepositories;
using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.SharedKernel.Results;

namespace PetFamily.Auth.Application.RoleManagement.Queries.GetRoles;

public class GetRolesQueryHandler(
    IRoleReadRepository roleReadRepository) : IQueryHandler<List<RoleDto>, GetRolesQuery>
{
    private readonly IRoleReadRepository _roleReadRepository = roleReadRepository;
    public async Task<Result<List<RoleDto>>> Handle(GetRolesQuery query, CancellationToken ct)
    {
        var roles = await _roleReadRepository.GetRoles(ct);

        return Result.Ok(roles);
    }
}
