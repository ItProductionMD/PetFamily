﻿using Dapper;
using PetFamily.Application.Abstractions;
using Volunteers.Public.IContracts;
using PetFamily.SharedInfrastructure.Shared.Dapper.ScaffoldedClasses;

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
                SELECT 1 FROM {PetTable.TableFullName}
                WHERE {PetTable.PetTypeBreedId} = @BreedId 
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
                SELECT 1 FROM {PetTable.TableFullName} 
                WHERE {PetTable.PetTypeSpeciesId} = @SpeciesId
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
