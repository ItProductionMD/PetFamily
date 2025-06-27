using PetFamily.Auth.Domain.Entities;
using PetFamily.SharedKernel.Results;

namespace PetFamily.Auth.Application.IRepositories;

public interface IRefreshTokenWriteRepository
{
    Task<Result<RefreshTokenSession>> GetRefreshToken(string token, CancellationToken ct = default);
    Task AddAsync(RefreshTokenSession refreshToken, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
