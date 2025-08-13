using Dapper;
using Microsoft.Extensions.Logging;
using PetFamily.SharedApplication.Abstractions;
using PetFamily.SharedInfrastructure.Dapper.ScaffoldedClasses;
using PetSpecies.Public.Dtos;
using PetSpecies.Public.IContracts;
using System.Collections.Generic;

namespace PetSpecies.Infrastructure.Contracts;

public class SpeciesQueryContract(
    IDbConnectionFactory dbConnectionFactory,
    ILogger<SpeciesExistenceContract> logger) : ISpeciesQueryContract
{
    private readonly IDbConnectionFactory _dbConnectionFactory = dbConnectionFactory;
    private readonly ILogger<SpeciesExistenceContract> _logger = logger;
    public async Task<List<SpeciesDto>> GetAllSpeciesAsync(CancellationToken ct = default)
    {
        await using var dbConnection = await _dbConnectionFactory.CreateOpenConnectionAsync();

        var sql = $@"
            SELECT s.{SpeciesTable.Id} AS Id,
                   s.{SpeciesTable.Name} AS Name,
            COALESCE
            (
                jsonb_agg
                (
                    jsonb_build_object
                    (
                        'BreedId', b.{BreedsTable.Id},'BreedName', b.{BreedsTable.Name}
                    )
                ) 
                FILTER (WHERE b.{BreedsTable.Id} IS NOT NULL), '[]'::jsonb
            ) AS BreedDtos
            FROM {SpeciesTable.TableFullName} s
            LEFT JOIN {BreedsTable.TableFullName} b ON b.{BreedsTable.SpeciesId} = s.{SpeciesTable.Id}
            GROUP BY s.{SpeciesTable.Id}, s.{SpeciesTable.Name}
            ORDER BY s.{SpeciesTable.Name}";
          
        _logger.LogInformation("EXECUTE(GetAllSpecies) SQL: {sql}", sql);

        var speciesList = await dbConnection.QueryAsync<SpeciesDto>(sql);

        return speciesList.ToList();
    }

}
