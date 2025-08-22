using Authorization.Application.Dtos;
using Authorization.Application.IRepositories.IAuthorizationRepo;
using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.SharedKernel.Results;

namespace Authorization.Application.RoleManagement.Queries.GetRoles;

public class GetRolesQueryHandler(IRoleReadRepo roleReadRepo) 
    : IQueryHandler<List<RoleDto>, GetRolesQuery>
{
    public async Task<Result<List<RoleDto>>> Handle(GetRolesQuery query, CancellationToken ct)
    {
        var roles = await roleReadRepo.GetRoles(ct);

        return Result.Ok(roles);
    }
}