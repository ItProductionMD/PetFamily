using Account.Application.Dtos;
using Account.Domain.ValueObjects;
using Account.Public.Dtos;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.ValueObjects;

namespace Account.Application.IRepositories;

public interface IUserReadRepository
{
    Task<Result<UserDto>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<UnitResult> CheckUniqueFields(string email, string login, Phone phone, CancellationToken ct = default);
    Task<bool> AnyUserWithRoleAsync(RoleId roleId, CancellationToken ct = default);
    Task<Result<UserAccountInfoDto>> GetUserAccountInfo(Guid userId, CancellationToken ct = default);
}
