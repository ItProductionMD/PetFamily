using Account.Application.IRepositories;
using Account.Infrastructure.Contexts;
using PetFamily.SharedInfrastructure.Shared;

namespace Account.Infrastructure.Repository;


public class UserAccountUnitOfWork(UserWriteDbContext context)
    : UnitOfWork<UserWriteDbContext>(context), IUserAccountUnitOfWork
{
}

