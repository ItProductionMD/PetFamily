
using Microsoft.EntityFrameworkCore.Storage;
using PetFamily.Application;
using System.Data;

namespace PetFamily.Infrastructure.Repositories;

public class UnitOfWork(AppDbContext dbContext) : IUnitOfWork
{
    private readonly AppDbContext _dbContext = dbContext;
    public async Task<IDbTransaction> BeginTransaction(CancellationToken cancelToken = default)
    {
        var transaction = await _dbContext.Database.BeginTransactionAsync(cancelToken);
        return transaction.GetDbTransaction();
    }

    public async Task SaveChangesAsync(CancellationToken cancelToken = default)
    {
        await _dbContext.SaveChangesAsync(cancelToken);
    }
}
