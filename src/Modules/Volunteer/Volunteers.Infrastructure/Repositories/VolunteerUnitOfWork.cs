using PetFamily.SharedInfrastructure.Shared;
using Volunteers.Application.IRepositories;
using Volunteers.Infrastructure.Contexts;

namespace Volunteers.Infrastructure.Repositories;

public class VolunteerUnitOfWork(VolunteerWriteDbContext context)
    : UnitOfWork<VolunteerWriteDbContext>(context), IVolunteerUnitOfWork
{
}
