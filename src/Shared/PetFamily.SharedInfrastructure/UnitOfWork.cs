using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PetFamily.SharedApplication.IUnitOfWork;

namespace PetFamily.SharedInfrastructure.Shared;

public class UnitOfWork<T>(T dbContext) : IUnitOfWork where T : DbContext
{
    private readonly T _dbContext = dbContext;
    private IDbContextTransaction? _currentTransaction;

    public async Task BeginTransactionAsync(CancellationToken ct = default)
    {
        if (_currentTransaction != null) return;
        _currentTransaction = await _dbContext.Database.BeginTransactionAsync(ct);
    }
    public async Task CommitAsync(CancellationToken ct = default)
    {
        if (_currentTransaction == null) return;
        await _dbContext.SaveChangesAsync(ct);
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
        return _dbContext.SaveChangesAsync(ct);
    }
}
