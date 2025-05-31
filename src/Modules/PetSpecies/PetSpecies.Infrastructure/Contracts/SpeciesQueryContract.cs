using Dapper;
using Microsoft.Extensions.Logging;
using PetFamily.Application.Abstractions;
using PetFamily.SharedInfrastructure.Shared.Dapper.ScaffoldedClasses;
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
                        'BreedId', b.{BreedTable.Id},'BreedName', b.{BreedTable.Name}
                    )
                ) 
                FILTER (WHERE b.{BreedTable.Id} IS NOT NULL), '[]'::jsonb
            ) AS BreedDtos
            FROM {SpeciesTable.TableFullName} s
            LEFT JOIN {BreedTable.TableFullName} b ON b.{BreedTable.SpeciesId} = s.{SpeciesTable.Id}
            GROUP BY s.{SpeciesTable.Id}, s.{SpeciesTable.Name}
            ORDER BY s.{SpeciesTable.Name}";
          
        _logger.LogInformation("EXECUTE(GetAllSpecies) SQL: {sql}", sql);

        var speciesList = await dbConnection.QueryAsync<SpeciesDto>(sql);

        return speciesList.ToList();
    }

}
