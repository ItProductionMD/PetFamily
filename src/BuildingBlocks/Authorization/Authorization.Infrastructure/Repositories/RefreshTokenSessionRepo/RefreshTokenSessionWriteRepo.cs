using Authorization.Application.IRepositories.IRefreshTokenSessionRepo;
using Authorization.Domain.Entities;
using Authorization.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;

namespace Authorization.Infrastructure.Repositories.RefreshTokenSessionRepo;

public class RefreshTokenWriteRepo(
   JwtTokenWriteDbContext context,
   ILogger<RefreshTokenSession> logger) : IRefreshTokenWriteRepo
{
    public async Task AddAsync(RefreshTokenSession refreshToken, CancellationToken ct = default)
    {
        await context.RefreshTokens.AddAsync(refreshToken, ct);
    }
    public async Task AddAndSaveAsync(RefreshTokenSession refreshToken, CancellationToken ct = default)
    {
        await context.RefreshTokens.AddAsync(refreshToken, ct);
        await context.SaveChangesAsync(ct);
        logger.LogInformation("RefreshToken with id: {Id} added successfully!", refreshToken.Id);
    }

    public async Task<Result<RefreshTokenSession>> GetRefreshTokenAsync(string token, CancellationToken ct = default)
    {
        var refreshTokenSession = await context.RefreshTokens
            .FirstOrDefaultAsync(x => x.Token == token, ct);

        if (refreshTokenSession == null)
        {
            logger.LogWarning("Refresh token not found!");
            return Result.Fail(Error.NotFound("RefreshToken"));
        }

        return Result.Ok(refreshTokenSession);
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        await context.SaveChangesAsync(ct);
    }
}
