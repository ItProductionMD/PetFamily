using Authorization.Application.IRepositories.IAuthorizationRepo;
using Authorization.Domain.Entities;
using Authorization.Infrastructure.Contexts;
using Microsoft.Extensions.Logging;
using PetFamily.SharedKernel.Results;

namespace Authorization.Infrastructure.Repositories.AuthorizationRepo;
public class PermissionWriteRepo(
    AuthorizationWriteDbContext context,
    ILogger<PermissionWriteRepo> logger) : IPermissionWriteRepo
{
    public async Task<Result<Guid>> AddPermissionAsync(Permission permission, CancellationToken ct = default)
    {
        await context.Permissions.AddAsync(permission, ct);

        await context.SaveChangesAsync(ct);

        logger.LogInformation("Permission with Id:{} was added successfully!", permission.Id);

        return Result.Ok(permission.Id);
    }

    public async Task AddPermissions(IEnumerable<Permission> permissions, CancellationToken ct = default)
    {
        await context.Permissions.AddRangeAsync(permissions, ct);
    }

    public async Task<UnitResult> SaveAsync(CancellationToken ct = default)
    {
        await context.SaveChangesAsync(ct);

        return UnitResult.Ok();
    }
}

