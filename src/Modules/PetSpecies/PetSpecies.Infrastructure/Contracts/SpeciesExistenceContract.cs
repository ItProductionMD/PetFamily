using Dapper;
using Microsoft.Extensions.Logging;
using PetFamily.SharedApplication.Abstractions;
using PetFamily.SharedInfrastructure.Dapper.ScaffoldedClasses;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using PetSpecies.Public.IContracts;

namespace PetSpecies.Infrastructure.Contracts;

public class SpeciesExistenceContract(
    ILogger<SpeciesExistenceContract> logger,
    IDbConnectionFactory dbConnectionFactory) : ISpeciesExistenceContract
{
    private readonly IDbConnectionFactory _dbConnectionFactory = dbConnectionFactory;
    private readonly ILogger<SpeciesExistenceContract> _logger = logger;

    public async Task<UnitResult> VerifySpeciesAndBreedExist(
        Guid speciesId,
        Guid breedId,
        CancellationToken cancelToken = default)
    {
        await using var dbConnection = await _dbConnectionFactory.CreateOpenConnectionAsync();

        var sql = $@"
            SELECT 1 
            FROM {BreedsTable.TableFullName} 
            WHERE {BreedsTable.SpeciesId} = @SpeciesId 
            AND {BreedsTable.Id} = @BreedId
            LIMIT 1";

        _logger.LogInformation("EXECUTE(CheckIfPetTypeExists) for speciesId:{speciesId} and breedId:" +
            "{breedId}. SQL: {sql}",
            speciesId, breedId, sql);

        var hasPetType = await dbConnection.QueryFirstOrDefaultAsync<int?>(
            sql,
            new { SpeciesId = speciesId, BreedId = breedId });

        if (hasPetType == null)
        {
            _logger.LogWarning("Pet type with speciesId:{speciesId} and breedId:{breedId} not exists!",
                speciesId, breedId);

            return UnitResult.Fail(Error.NotFound($"Pet type with speciesId:{speciesId}" +
                $" and breedId:{breedId}"));
        }
        _logger.LogInformation("Pet type with speciesId:{speciesId} and breedId:{breedId} exists!",
            speciesId, breedId);

        return UnitResult.Ok();
    }
}
