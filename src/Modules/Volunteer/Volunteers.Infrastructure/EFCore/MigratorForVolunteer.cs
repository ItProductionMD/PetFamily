using Microsoft.EntityFrameworkCore;
using PetFamily.SharedInfrastructure.Shared.EFCore;
using Volunteers.Infrastructure.Contexts;

namespace Volunteers.Infrastructure.EFCore;

public class MigratorForVolunteer : IDbMigrator
{
    private readonly VolunteerWriteDbContext _dbContext;

    public MigratorForVolunteer(VolunteerWriteDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task MigrateAsync() => await _dbContext.Database.MigrateAsync();
}
