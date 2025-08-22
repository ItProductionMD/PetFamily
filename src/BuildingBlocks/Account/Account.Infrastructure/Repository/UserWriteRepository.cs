using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Account.Application.IRepositories;
using Account.Domain.Entities.UserAggregate;
using Account.Infrastructure.Contexts;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.ValueObjects.Ids;

namespace Account.Infrastructure.Repository;

public class UserWriteRepository : IUserWriteRepository
{
    private readonly ILogger<UserWriteRepository> _logger;
    private readonly UserWriteDbContext _context;
    public UserWriteRepository(
        ILogger<UserWriteRepository> logger,
        UserWriteDbContext context)
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

    public async Task AddAndSaveAsync(User user, CancellationToken ct)
    {
        await _context.Users.AddAsync(user, ct);
        await _context.SaveChangesAsync(ct);
    }

    Task<Result<User>> IUserWriteRepository.GetByEmailAsync(string email, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
