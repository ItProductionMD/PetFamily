using Microsoft.EntityFrameworkCore.Storage;
using PetFamily.Auth.Application.IRepositories;
using PetFamily.Auth.Infrastructure.Contexts;
using PetFamily.SharedInfrastructure.Shared;

namespace PetFamily.Auth.Infrastructure.Repository;


public class AuthUnitOfWork(AuthWriteDbContext context)
    : UnitOfWork<AuthWriteDbContext>(context), IAuthUnitOfWork
{
}

