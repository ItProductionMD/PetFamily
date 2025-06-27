using PetFamily.Auth.Application.Dtos;
using PetFamily.Auth.Domain.Entities;
using PetFamily.Auth.Domain.Entities.UserAggregate;
using PetFamily.Auth.Domain.ValueObjects;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.ValueObjects;
using PetFamily.SharedKernel.ValueObjects.Ids;

namespace PetFamily.Auth.Application.IRepositories;

public interface IUserReadRepository
{
    Task<Result<UserDto>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<UnitResult> CheckUniqueFields(string email, string login, Phone phone, CancellationToken ct = default);
    Task<bool> AnyUserWithRoleAsync(RoleId roleId, CancellationToken ct = default);
}
