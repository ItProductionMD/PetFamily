using PetFamily.Auth.Domain.Entities.UserAggregate;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.ValueObjects.Ids;

namespace PetFamily.Auth.Application.IRepositories;

public interface IUserWriteRepository
{
    Task AddAsync(User user, CancellationToken ct);
    Task UpdateAsync(User user, CancellationToken ct);
    Task DeleteAsync(Guid id, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
    Task<Result<User>> GetByIdAsync(UserId userId, CancellationToken ct);
    Task<User?> GetByEmailAsync(string email, CancellationToken ct);
}