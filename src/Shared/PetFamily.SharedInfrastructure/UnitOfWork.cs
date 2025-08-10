using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PetFamily.SharedApplication.Abstractions;
using System.Data;

namespace PetFamily.SharedInfrastructure.Shared;

public class UnitOfWork<T>(T dbContext) : IUnitOfWork where T : DbContext
{
    private readonly T _dbContext = dbContext;

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
