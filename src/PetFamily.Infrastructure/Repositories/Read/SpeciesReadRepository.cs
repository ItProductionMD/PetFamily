using Amazon.Runtime.Internal.Util;
using Dapper;
using Microsoft.Extensions.Logging;
using PetFamily.Application.IRepositories;
using PetFamily.Domain.DomainError;
using PetFamily.Domain.Results;
using PetFamily.Infrastructure.Dapper.GeneratedTables;
using SpeciesTable = PetFamily.Infrastructure.Dapper.GeneratedTables.Species;
using Species = PetFamily.Domain.PetTypeManagment.Root.Species;
using Breed = PetFamily.Domain.PetTypeManagment.Entities.Breed;
using System.Data;
using System.Security.Cryptography;
using PetFamily.Domain.PetManagment.ValueObjects;
using PetFamily.Infrastructure.Contexts.ReadDbContext.Models;
using Microsoft.Extensions.Options;
using PetFamily.Infrastructure.Dapper;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using PetFamily.Application.Queries.PetType.GetListOfSpecies;
using PetFamily.Application.Queries.PetType.GetBreeds;
using PetFamily.Application.Abstractions;
using PetFamily.Application.Dtos;

namespace PetFamily.Infrastructure.Repositories.Read;

public class SpeciesReadRepository(
    IDbConnectionFactory dbConnectionFactory,
    IOptions<DapperOptions> options,
    ILogger<SpeciesReadRepository> logger) : ISpeciesReadRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory = dbConnectionFactory;
    private readonly DapperOptions _options = options.Value;
    private readonly ILogger<SpeciesReadRepository> _logger = logger;

    public async Task<UnitResult> CheckForDeleteAsync(
        Guid speciesId, 
        CancellationToken cancelToken = default)
    {
        using var dbConnection = _dbConnectionFactory.CreateConnection();
        var sql = $@"
            SELECT 1 
            FROM {Pets.Table} 
            WHERE {Pets.PetTypeSpeciesId} = @SpeciesId
            LIMIT 1";

        _logger.LogInformation("EXECUTE(CheckForDeletingAsync) for species,check if exists at least" +
            " one pet with species Id:{speciesId}. SQL: {sql}",
            speciesId, sql);

        var hasPet = await dbConnection.QueryFirstOrDefaultAsync<int?>(
            sql,
            new { SpeciesId = speciesId },
            commandTimeout: _options.QueryTimeout);

        if (hasPet.HasValue)
        {
            _logger.LogWarning("Deletion of species with id:{Id} prevented - species is in use by pets.",
                speciesId);

            return UnitResult.Fail(Error.InternalServerError($"Cannot delete species with Id:{speciesId}, " +
                $"because it is used by one or more pets."));
        }

        _logger.LogInformation("Species with id:{Id} can be deleted.", speciesId);
        return UnitResult.Ok();
    }

    public async Task<UnitResult> CheckIfPetTypeExists(
        Guid speciesId,
        Guid breedId,
        CancellationToken cancelToken = default)
    {
        using var dbConnection = _dbConnectionFactory.CreateConnection();

        var sql = $@"
            SELECT 1 
            FROM {Breeds.Table} 
            WHERE {Breeds.SpeciesId} = @SpeciesId 
            AND {Breeds.Id} = @BreedId
            LIMIT 1";

        _logger.LogInformation("EXECUTE(CheckIfPetTypeExists) for speciesId:{speciesId} and breedId:{breedId}. SQL: {sql}",
            speciesId, breedId, sql);

        var hasPetType = await dbConnection.QueryFirstOrDefaultAsync<int?>(
            sql,
            new { SpeciesId = speciesId, BreedId = breedId },
            commandTimeout: _options.QueryTimeout);

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

    public async Task<UnitResult> CheckForDeleteBreedAsync(
        Guid breedId,
        CancellationToken cancelToken = default)
    {
        using var dbConnection = _dbConnectionFactory.CreateConnection();

        var sql = $@"
            SELECT 1 
            FROM {Pets.Table} 
            WHERE {Pets.PetTypeBreedId} = @BreedId
            LIMIT 1";

        _logger.LogInformation("EXECUTE(CheckForDeleteBreedAsync),check if exists at least" +
            " one pet with breedId:{Id}. SQL: {sql}", breedId, sql);

        var hasPet = await dbConnection.QueryFirstOrDefaultAsync<int?>(
            sql,
            new { BreedId = breedId },
            commandTimeout: _options.QueryTimeout);

        if (hasPet.HasValue)
        {
            _logger.LogWarning("Deletion of breed with id:{Id} prevented - breed is in use by pets.",
                breedId);

            return UnitResult.Fail(Error.InternalServerError($"Cannot delete breed with Id:{breedId}, " +
                $"because it is used by one or more pets."));
        }

        _logger.LogInformation("Breed with id:{Id} can be deleted.", breedId);

        return UnitResult.Ok();
    }

    public async Task<Result<Breed>> GetBreedByIdAsync(
        Guid breedId, 
        CancellationToken cancelToken = default)
    {
        using var dbConnection = _dbConnectionFactory.CreateConnection();

        var sql = $@"
                SELECT b.{Breeds.Id} AS Id,
                       b.{Breeds.Name} AS Name,
                       b.{Breeds.Description} AS Description
                FROM {Breeds.Table} b
                WHERE b.Id = @BreedId
                LIMIT 1";

        _logger.LogInformation("EXECUTING DAPPER 'GetBreedById' with breedId:{Id} SQL:{sql}",
            breedId, sql);

        var breed = await dbConnection.QuerySingleOrDefaultAsync<Breed>(
            sql,
            new { BreedId = breedId },
            commandTimeout: _options.QueryTimeout);

        if (breed == null)
        {
            _logger.LogWarning("Breed with id:{Id} not found!", breedId);
            return UnitResult.Fail(Error.NotFound($"Breed with id:{breedId} "));
        }
        _logger.LogInformation("Get breed with id:{Id} succesfull!", breedId);

        return Result.Ok(breed);
    }

    public async Task<GetBreedsResponse> GetBreedsAsync(
        GetBreedsQuery query,
        CancellationToken cancelToken = default)
    {
        using var dbConnection = _dbConnectionFactory.CreateConnection();

        var sql = $@"
            SELECT b.{Breeds.Id} AS Id,
                   b.{Breeds.Name} As Name,
                   b.{Breeds.Description} As Description
            FROM {Breeds.Table} b
            WHERE b.{Breeds.SpeciesId} = @SpeciesId
            ORDER BY b.{Breeds.Name} {query.SortDirection} 
            LIMIT @Limit OFFSET @Offset";

        var offset = (query.Page - 1) * query.PageSize;
        var limit = query.PageSize + 1;

        _logger.LogInformation("EXECUTING(GetBreedsAsync) for speciesId:{Id} with SQL:{sql}",
            query.SpeciesId, sql);

        var breeds = await dbConnection.QueryAsync<Breed>(
            sql,
            new { SpeciesId = query.SpeciesId , Limit = limit, Offset = offset },
            commandTimeout: _options.QueryTimeout);

        var breedsList = breeds.ToList();
        bool hasMoreRecords = breedsList.Count > query.PageSize;

        var breedsCount = (query.Page - 1) * query.PageSize + breedsList.Count;
        if (hasMoreRecords)
        {
            breedsList.RemoveAt(breedsList.Count - 1);

            var getSpeciesCountSql = $@"SELECT COUNT(1) FROM {Breeds.Table}";

            _logger.LogInformation("Executing(GetBreedsAsync) by species with id:{Id} SQL Query:{sql}", 
                query.SpeciesId, getSpeciesCountSql);

            breedsCount = await dbConnection.ExecuteScalarAsync<int>(
                getSpeciesCountSql,
                commandTimeout: _options.QueryTimeout);
        }
        _logger.LogInformation("Get breeds for species with id:{Id} ok!", query.SpeciesId);

        return new(breedsList.Count, breedsList);
    }

    public async Task<Result<Species>> GetByIdAsync(
        Guid speciesId, 
        CancellationToken cancelToken = default)
    {
        using var dbConnection = _dbConnectionFactory.CreateConnection();

        var sql = $@"
            SELECT s.{SpeciesTable.Id} AS Id
                   s.{SpeciesTable.Name} AS Name
                   COALESCE(
                       JSON_AGG(
                           JSONB_BUILD_OBJECT(
                               'Id', b.{Breeds.Id}, 
                               'Name', b.{Breeds.Name},
                               'Description', b.{Breeds.Description})
                           ORDER BY b.name
                        ) FILTER (WHERE b.{Breeds.Id} IS NOT NULL),
                        '[]'
                    ) AS Breeds
            FROM {SpeciesTable.Table} s
            WHERE s.{SpeciesTable.Id} = @SpeciesId
            LEFT JOIN {Breeds.Table} b ON s.{SpeciesTable.Id} = b.{Breeds.SpeciesId}
            GROUP BY s.{SpeciesTable.Id}, s.{SpeciesTable.Name} 
            ORDER BY s.{SpeciesTable.Name}
            LIMIT 1";

        _logger.LogInformation("EXECUTING DAPPER GetSpeciesByIdAsync speciesId:{Id} with SQL:{sql} with sp ",
            speciesId, sql);

        var species = await dbConnection.QuerySingleAsync<Species>(sql, new { SpeciesId = speciesId });
        if (species == null)
        {
            _logger.LogWarning("Species with id:{Id} not found!", speciesId);
            return UnitResult.Fail(Error.NotFound($"Species with id:{speciesId}"));
        }
        _logger.LogInformation("Get species with id{Id} ok!", speciesId);

        return Result.Ok(species);
    }

    public async Task<GetListOfSpeciesResponse> GetListOfSpeciesAsync(
        GetListOfSpeciesQuery query,
        CancellationToken cancelToken = default)
    {
        using var dbConnection = _dbConnectionFactory.CreateConnection();

        var orderBy = query.SortBy;
        var sql = $@"
            SELECT s.{SpeciesTable.Id} AS Id,
                   s.{SpeciesTable.Name} AS Name
            FROM {SpeciesTable.Table} s
            ORDER BY s.{orderBy} {query.SortDirection}
            LIMIT @Limit OFFSET @Offset";

        var offset = (query.Page - 1) * query.PageSize;
        var limit = query.PageSize + 1;

        _logger.LogInformation("EXECUTING(GetSpeciesesAsync) with SQL:{sql} ", sql);

        var species = await dbConnection.QueryAsync<Species>(
            sql,
            new { Limit = limit, Offset = offset },
            commandTimeout: _options.QueryTimeout);

        var speciesList = species.ToList();

        bool hasMoreRecords = speciesList.Count > query.PageSize;

        var speciesCount = (query.Page - 1) * query.PageSize + speciesList.Count;
        if (hasMoreRecords)
        {
            speciesList.RemoveAt(speciesList.Count - 1);

            var getSpeciesCountSql = $@"SELECT COUNT(1) FROM {SpeciesTable.Table}";

            _logger.LogInformation("Executing(GetListOfSpecies) SQL Query:{sql}", getSpeciesCountSql);

            speciesCount = await dbConnection.ExecuteScalarAsync<int>(
                getSpeciesCountSql,
                commandTimeout: _options.QueryTimeout);
        }
        _logger.LogInformation("Get all species ok! Species count:{count}", species.Count());

        return new(speciesCount, speciesList);
    }

    public async Task<Result<List<SpeciesDto>>> GetSpeciesDtos(CancellationToken cancellationToken = default)
    {
        var sql = $@"
            SELECT s.{SpeciesTable.Id} AS Id,
                   s.{SpeciesTable.Name} AS Name,
                   COALESCE(
                       JSON_AGG(
                           JSONB_BUILD_OBJECT(
                               'Id', b.{Breeds.Id}, 
                               'Name', b.{Breeds.Name})
                           ORDER BY b.name
                        ) FILTER (WHERE b.{Breeds.Id} IS NOT NULL),
                        '[]'
                    ) AS BreedDtos
            FROM {SpeciesTable.Table} s
            LEFT JOIN {Breeds.Table} b ON s.{SpeciesTable.Id} = b.{Breeds.SpeciesId}
            GROUP BY s.{SpeciesTable.Id}, s.{SpeciesTable.Name} 
            ORDER BY s.{SpeciesTable.Name}";

        _logger.LogInformation("EXECUTING DAPPER GetSpeciesDtos with SQL:{sql}", sql);

        using var connection = _dbConnectionFactory.CreateConnection();

        var speciesDtos = await connection.QueryAsync<SpeciesDto>(sql,_options.QueryTimeout);

        if (speciesDtos == null)
        {
            _logger.LogWarning("No species found!");
            return UnitResult.Fail(Error.NotFound($"No species found!"));
        }
        _logger.LogInformation("Get all species ok!");

        return Result.Ok(speciesDtos.ToList());
    }
}
