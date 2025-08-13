using Microsoft.Extensions.Logging;
using PetFamily.Auth.Application.IRepositories;
using PetFamily.Auth.Domain.Entities;
using PetFamily.Auth.Infrastructure.Contexts;
using PetFamily.SharedKernel.Results;

namespace PetFamily.Auth.Infrastructure.Repository;

public class PermissionWriteRepository(
    AuthWriteDbContext context,
    ILogger<PermissionWriteRepository> logger) : IPermissionWriteRepository
{
    private readonly AuthWriteDbContext _context = context;
    private readonly ILogger<PermissionWriteRepository> _logger = logger;

    public async Task<Result<Guid>> AddPermissionAsync(Permission permission, CancellationToken ct = default)
    {
        await _context.Permissions.AddAsync(permission, ct);

        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Permission with Id:{} was added successfully!", permission.Id);

        return Result.Ok(permission.Id.Value);
    }

    public async Task AddPermissions(IEnumerable<Permission> permissions, CancellationToken ct = default)
    {
        await _context.Permissions.AddRangeAsync(permissions, ct);
    }

    public async Task<UnitResult> SaveAsync(CancellationToken ct = default)
    {
        await _context.SaveChangesAsync(ct);

        return UnitResult.Ok();
    }
}
