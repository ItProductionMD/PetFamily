using Dapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PetFamily.Application.Abstractions;
using PetFamily.Application.Dtos;
using PetFamily.SharedInfrastructure.Dapper.ScaffoldedClasses;
using PetFamily.SharedInfrastructure.Shared.Dapper;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using PetSpecies.Application.IRepositories;
using PetSpecies.Application.Queries.GetBreedPagedList;
using PetSpecies.Application.Queries.GetSpeciesPagedList;
using PetSpecies.Domain;
using PetSpecies.Public.Dtos;
using PetSpecies.Public.IContracts;

namespace PetSpecies.Infrastructure.Repositories.Read;

public class SpeciesReadRepository(
    IDbConnectionFactory dbConnectionFactory,
    IOptions<DapperOptions> options,
    ILogger<SpeciesReadRepository> logger) : ISpeciesReadRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory = dbConnectionFactory;
    private readonly DapperOptions _options = options.Value;
    private readonly ILogger<SpeciesReadRepository> _logger = logger;

    private readonly Dictionary<SpeciesSortProperty, string> SpeciesSortDict =
        new() { [SpeciesSortProperty.name] = SpeciesTable.Name };

    private readonly Dictionary<SpeciesSearchProperty, string> SpeciesSearchDict =
        new() { [SpeciesSearchProperty.name] = SpeciesTable.Name };

    public async Task<UnitResult> CheckForDeleteAsync(
        Guid speciesId,
        CancellationToken cancelToken = default)
    {
        using var dbConnection = _dbConnectionFactory.CreateConnection();
        var sql = $@"
            SELECT 1 
            FROM {PetsTable.TableFullName} 
            WHERE {PetsTable.PetTypeSpeciesId} = @SpeciesId
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

            return UnitResult.Fail(Error.Conflict($"Cannot delete species with Id:{speciesId}, " +
                $"because it is used by one or more pets."));
        }

        _logger.LogInformation("Species with id:{Id} can be deleted.", speciesId);
        return UnitResult.Ok();
    }

    public async Task<UnitResult> VerifySpeciesAndBreedExist(
        Guid speciesId,
        Guid breedId,
        CancellationToken cancelToken = default)
    {
        using var dbConnection = _dbConnectionFactory.CreateConnection();

        var sql = $@"
            SELECT 1 
            FROM {BreedsTable.TableFullName} 
            WHERE {BreedsTable.SpeciesId} = @SpeciesId 
            AND {BreedsTable.Id} = @BreedId
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
            FROM {PetsTable.TableFullName} 
            WHERE {PetsTable.PetTypeBreedId} = @BreedId
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
                SELECT b.{BreedsTable.Id} AS Id,
                       b.{BreedsTable.Name} AS Name,
                       b.{BreedsTable.Description} AS Description
                FROM {BreedsTable.TableFullName} b
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
                               'Id', b.{BreedsTable.Id}, 
                               'Name', b.{BreedsTable.Name},
                               'Description', b.{BreedsTable.Description})
                           ORDER BY b.name
                        ) FILTER (WHERE b.{BreedsTable.Id} IS NOT NULL),
                        '[]'
                    ) AS Breeds
            FROM {SpeciesTable.TableFullName} s
            WHERE s.{SpeciesTable.Id} = @SpeciesId
            LEFT JOIN {BreedsTable.TableFullName} b ON s.{SpeciesTable.Id} = b.{BreedsTable.SpeciesId}
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

    public async Task<SpeciesPagedListDto> GetSpeciesPagedList(
        SpeciesFilterDto? speciesFilter,
        PaginationDto? pagination,
        CancellationToken cancelToken = default)
    {
        var sortByParams = speciesFilter.GetOrderByes();

        Dictionary<string, string> SortByPropertyDirection = new();

        foreach (var itemOrderBy in sortByParams)
        {
            var sortBy = itemOrderBy.Property;
            var sortDirection = itemOrderBy.Direction.ToString().ToLower();
            SortByPropertyDirection.Add(SpeciesSortDict[sortBy], sortDirection);
        }

        var offset = (pagination.Page - 1) * pagination.PageSize;
        var limit = pagination.PageSize + 1;

        var orderBy = SortByPropertyDirection.FirstOrDefault().Key ?? SpeciesTable.Name;
        var orderDirection = SortByPropertyDirection.FirstOrDefault().Value ?? "asc";

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
            ORDER BY s.{orderBy} {orderDirection}
            LIMIT {limit} OFFSET {offset}";

        var countSql = $@"
            SELECT COUNT(*)
            FROM {SpeciesTable.TableFullName}";

        _logger.LogInformation("EXECUTING GET_SPECIES_PAGED_LIST with sql:{sql}", sql);
        _logger.LogInformation("EXECUTING GET_SPECIES_PAGED_LIST with count sql:{sql}", countSql);

        await using var conn1 = await _dbConnectionFactory.CreateOpenConnectionAsync();
        await using var conn2 = await _dbConnectionFactory.CreateOpenConnectionAsync();

        var speciesTask = conn1.QueryAsync<SpeciesDto>(sql);
        var countTask = conn2.ExecuteScalarAsync<int>(countSql);

        await Task.WhenAll(speciesTask, countTask);

        var species = speciesTask.Result;
        var totalCount = countTask.Result;

        return new(totalCount, species.ToList());
    }

    public async Task<BreedPagedListDto> GetBreedPagedList(
        BreedFilterDto breedFilter,
        PaginationDto pagination,
        CancellationToken cancelToken = default)
    {


        throw new Exception();
    }
}

