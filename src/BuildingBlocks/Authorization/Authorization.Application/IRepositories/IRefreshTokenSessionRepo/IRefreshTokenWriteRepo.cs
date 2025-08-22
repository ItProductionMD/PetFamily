using Authorization.Domain.Entities;
using PetFamily.SharedKernel.Results;

namespace Authorization.Application.IRepositories.IRefreshTokenSessionRepo;

public interface IRefreshTokenWriteRepo
{
    Task<Result<RefreshTokenSession>> GetRefreshTokenAsync(string token, CancellationToken ct = default);
    Task AddAsync(RefreshTokenSession refreshToken, CancellationToken ct = default);
    Task AddAndSaveAsync(RefreshTokenSession refreshToken, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}