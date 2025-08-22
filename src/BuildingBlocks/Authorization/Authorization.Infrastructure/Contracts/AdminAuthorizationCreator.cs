using Authorization.Application.IRepositories.IAuthorizationRepo;
using Authorization.Domain.Entities;
using Authorization.Infrastructure.Contexts;
using Authorization.Public.Contracts;
using PetFamily.SharedInfrastructure.Dapper.ScaffoldedClasses;
using PetFamily.SharedKernel.Authorization;
using PetFamily.SharedKernel.Results;

namespace Authorization.Infrastructure.Contracts;

internal class AdminAuthorizationCreator(
    IRoleReadRepo roleReadRepo, 
    AuthorizationWriteDbContext authorizationWriteDbContext)
    : IAdminAuthorizationCreator
{
    public async Task<UnitResult> CreateAdminAuthorization(Guid adminId, CancellationToken ct)
    {
        var adminRoleResult = await roleReadRepo.GetByCodeAsync(RoleCodes.ADMIN, ct);
        if (adminRoleResult.IsFailure)
            return UnitResult.Fail(adminRoleResult.Error);

        var role = adminRoleResult.Data!;

        var userRole = new UserRole(adminId, role.RoleId);

        authorizationWriteDbContext.UserRoles.Add(userRole);

        await authorizationWriteDbContext.SaveChangesAsync(ct);

        return UnitResult.Ok();
    }
}
