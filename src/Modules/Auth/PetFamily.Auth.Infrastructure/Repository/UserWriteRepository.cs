using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PetFamily.Auth.Application.IRepositories;
using PetFamily.Auth.Domain.Entities.UserAggregate;
using PetFamily.Auth.Infrastructure.Contexts;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.ValueObjects.Ids;

namespace PetFamily.Auth.Infrastructure.Repository;

public class UserWriteRepository : IUserWriteRepository
{
    private readonly ILogger<UserWriteRepository> _logger;
    private readonly AuthWriteDbContext _context;
    public UserWriteRepository(
        ILogger<UserWriteRepository> logger,
        AuthWriteDbContext context)
    {
        _logger = logger;
        _context = context;
    }
    public Task DeleteAsync(Guid id, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<User>> GetByIdAsync(UserId userId, CancellationToken ct)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId, ct);
        if (user == null)
        {
            _logger.LogWarning("User with id {UserId} not found", userId);

            return Result.Fail(Error.NotFound($"User with id {userId}"));
        }

        return Result.Ok(user);
    }

    public async Task SaveChangesAsync(CancellationToken ct)
    {
        await _context.SaveChangesAsync(ct);
    }

    public Task UpdateAsync(User user, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public async Task AddAsync(User user, CancellationToken ct)
    {
        await _context.Users.AddAsync(user, ct);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email, ct);
    }
}
