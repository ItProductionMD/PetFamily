using Microsoft.EntityFrameworkCore.Storage;
using PetFamily.Auth.Application.IRepositories;
using PetFamily.Auth.Infrastructure.Contexts;

namespace PetFamily.Auth.Infrastructure.Repository;


public class AuthUnitOfWork(AuthWriteDbContext context) : IAuthUnitOfWork
{
    private readonly AuthWriteDbContext _context = context;
    private IDbContextTransaction? _currentTransaction;

    public async Task BeginTransactionAsync(CancellationToken ct = default)
    {
        if (_currentTransaction != null) return;
        _currentTransaction = await _context.Database.BeginTransactionAsync(ct);
    }

    public async Task CommitAsync(CancellationToken ct = default)
    {
        if (_currentTransaction == null) return;
        await _context.SaveChangesAsync(ct);
        await _currentTransaction.CommitAsync(ct);
        await _currentTransaction.DisposeAsync();
        _currentTransaction = null;
    }

    public async Task RollbackAsync(CancellationToken ct = default)
    {
        if (_currentTransaction == null) return;
        await _currentTransaction.RollbackAsync(ct);
        await _currentTransaction.DisposeAsync();
        _currentTransaction = null;
    }

    public Task SaveChangesAsync(CancellationToken ct = default)
    {
        return _context.SaveChangesAsync(ct);
    }
}

