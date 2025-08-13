using Dapper;
using PetFamily.SharedApplication.Abstractions;
using PetFamily.SharedInfrastructure.Dapper.ScaffoldedClasses;
using Volunteers.Public.IContracts;

namespace Volunteers.Infrastructure.Contracts
{
    public class PetExistenceContract(
        IDbConnectionFactory dbConnectionFactory) : IPetExistenceContract
    {
        private readonly IDbConnectionFactory _dbConnectionFactory = dbConnectionFactory;

        public async Task<bool> ExistsWithBreedAsync(Guid breedId, CancellationToken ct = default)
        {
            await using var dbConnection = await _dbConnectionFactory.CreateOpenConnectionAsync();

            const string sql = $@"
                SELECT 1 FROM {PetsTable.TableFullName}
                WHERE {PetsTable.PetTypeBreedId} = @BreedId 
                LIMIT 1;";

            var result = await dbConnection.QueryFirstOrDefaultAsync<int?>(new CommandDefinition(
                sql,
                new { BreedId = breedId },
                cancellationToken: ct
            ));

            return result.HasValue;
        }

        public async Task<bool> ExistsWithSpeciesAsync(Guid speciesId, CancellationToken ct = default)
        {
            await using var dbConnection = await _dbConnectionFactory.CreateOpenConnectionAsync();

            const string sql = $@"
                SELECT 1 FROM {PetsTable.TableFullName} 
                WHERE {PetsTable.PetTypeSpeciesId} = @SpeciesId
                LIMIT 1;";

            var result = await dbConnection.QueryFirstOrDefaultAsync<int?>(new CommandDefinition(
                sql,
                new { SpeciesId = speciesId },

                cancellationToken: ct
            ));

            return result.HasValue;
        }
    }
}
