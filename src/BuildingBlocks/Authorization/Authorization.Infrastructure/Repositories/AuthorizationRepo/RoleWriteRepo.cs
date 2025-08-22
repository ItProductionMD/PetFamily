using Authorization.Application.IRepositories.IAuthorizationRepo;
using Authorization.Domain.Entities.RoleAggregate;
using Authorization.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;

namespace Authorization.Infrastructure.Repositories.AuthorizationRepo;

public class RoleWriteRepo(AuthorizationWriteDbContext context) : IRoleWriteRepo
{
    public Task<Result<Guid>> AddAndSaveAsync(Role role, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<Guid>> AddRole(Role role, CancellationToken ct = default)
    {
        await context.Roles.AddAsync(role, ct);
        return Result.Ok(role.Id);
    }

    public async Task AddRoles(IEnumerable<Role> roles, CancellationToken ct = default)
    {
        await context.Roles.AddRangeAsync(roles, ct);
    }

    public Task<UnitResult> DeleteRole(Guid roleId, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<Role>> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var role = await context.Roles
            .Include(r => r.RolePermissions)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

        if (role == null)
            return Result.Fail(Error.NotFound($"role with id: {id}"));

        return Result.Ok(role);
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        await context.SaveChangesAsync(ct);
    }
}
