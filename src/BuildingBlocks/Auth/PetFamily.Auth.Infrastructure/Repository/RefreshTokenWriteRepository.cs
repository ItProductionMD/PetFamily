
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PetFamily.Auth.Application.IRepositories;
using PetFamily.Auth.Domain.Entities;
using PetFamily.Auth.Infrastructure.Contexts;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;

namespace PetFamily.Auth.Infrastructure.Repository;

public class RefreshTokenWriteRepository(
    ILogger<RefreshTokenSession> logger,
    AuthWriteDbContext context) : IRefreshTokenWriteRepository
{
    private readonly AuthWriteDbContext _context = context;
    private readonly ILogger<RefreshTokenSession> _logger = logger;

    public async Task AddAsync(RefreshTokenSession refreshToken, CancellationToken ct = default)
    {
        await _context.RefreshTokens.AddAsync(refreshToken, ct);
    }

    public async Task<Result<RefreshTokenSession>> GetRefreshToken(string token, CancellationToken ct = default)
    {
        var refreshTokenSession = await _context.RefreshTokens
            .FirstOrDefaultAsync(x => x.Token == token, ct);

        if (refreshTokenSession == null)
        {
            _logger.LogWarning("Refresh token not found!");
            return Result.Fail(Error.NotFound("RefreshToken"));
        }

        return Result.Ok(refreshTokenSession);
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        await _context.SaveChangesAsync(ct);
    }
}
