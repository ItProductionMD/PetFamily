using Account.Public.Dtos;
using PetFamily.SharedKernel.Results;
using System.Security.Claims;

namespace Account.Public.Contracts;

public interface IUserContract
{
    Task<Result<UserDto>> GetByIdAsync(Guid userId, CancellationToken ct = default);
    Task<Result<List<Claim>>> GetUserClaims(Guid userId, CancellationToken ct = default);
}
