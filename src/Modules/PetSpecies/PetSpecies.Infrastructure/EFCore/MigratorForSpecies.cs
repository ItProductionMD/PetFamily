using Microsoft.EntityFrameworkCore;
using PetFamily.SharedInfrastructure.Shared.EFCore;
using PetSpecies.Infrastructure.Contexts;

namespace PetSpecies.Infrastructure.EFCore;

public class MigratorForSpecies : IDbMigrator
{
    private readonly SpeciesWriteDbContext _dbContext;

    public MigratorForSpecies(SpeciesWriteDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task MigrateAsync() => await _dbContext.Database.MigrateAsync();
}
