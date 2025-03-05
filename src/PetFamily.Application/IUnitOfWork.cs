using System.Data;

namespace PetFamily.Application;

public interface IUnitOfWork
{
    Task<IDbTransaction> BeginTransaction(CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
