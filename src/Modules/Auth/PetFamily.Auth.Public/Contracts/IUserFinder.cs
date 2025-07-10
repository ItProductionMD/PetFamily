using PetFamily.Auth.Public.Dtos;
using PetFamily.SharedKernel.Results;

namespace PetFamily.Auth.Public.Contracts;

public interface IUserFinder
{
    Task<Result<UserDto>> FindById(Guid userId, CancellationToken ct = default);
}
