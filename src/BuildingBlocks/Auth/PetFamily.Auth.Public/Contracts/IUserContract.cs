using PetFamily.Auth.Public.Dtos;
using PetFamily.SharedKernel.Results;

namespace PetFamily.Auth.Public.Contracts;

public interface IUserContract
{
    Task<Result<UserDto>> GetByIdAsync(Guid userId, CancellationToken ct = default);
}
