
using Microsoft.EntityFrameworkCore.Storage;
using PetFamily.Application.IRepositories;
using PetFamily.Infrastructure.Contexts;
using System.Data;

namespace PetFamily.Infrastructure.Repositories.Write;

public class UnitOfWork(WriteDbContext dbContext) : IUnitOfWork
{
    private readonly WriteDbContext _dbContext = dbContext;
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
