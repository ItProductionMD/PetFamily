using Dapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PetFamily.SharedApplication.Abstractions;
using PetFamily.SharedInfrastructure.Dapper.Extensions;
using PetFamily.SharedInfrastructure.Dapper.ScaffoldedClasses;
using PetFamily.SharedInfrastructure.Shared.Dapper;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using System.Text;
using Volunteers.Application.IRepositories;
using Volunteers.Application.Queries.GetPets;
using Volunteers.Application.Queries.GetPets.ForFilter;
using Volunteers.Application.Queries.GetVolunteers;
using Volunteers.Application.ResponseDtos;
using Volunteers.Infrastructure.Extensions;

using static PetFamily.SharedInfrastructure.Dapper.Extensions.DynamicParametersExtensions;


namespace Volunteers.Infrastructure.Repositories;

public class VolunteerReadRepository(
    IDbConnectionFactory dbConnectionFactory,
    IOptions<DapperOptions> options,
    ILogger<VolunteerReadRepository> logger) : IVolunteerReadRepository 
{
    private readonly IDbConnectionFactory _dbConnectionFactory = dbConnectionFactory;
    private readonly DapperOptions _options = options.Value;
    private readonly ILogger<VolunteerReadRepository> _logger = logger;

    public async Task<UnitResult> CheckUniqueFields(
        Guid volunteerId,
        string phone,
        CancellationToken cancellationToken = default)
    {
        var sql = $@"
        SELECT 
            CASE WHEN EXISTS (
                SELECT 1 
                FROM {VolunteersTable.TableFullName}
                WHERE {VolunteersTable.Phone} = @Phone
                AND {VolunteersTable.Id} <> @Id) 
                THEN 'Phone' ELSE NULL END AS PhoneTaken;";

        _logger.LogInformation("EXECUTING QUERY(CheckUniqueFields) SQL Query: {sql} with Parameters:" +
            " {volunteerId}, {phone}",
            sql, volunteerId, phone);

        await using var dbConnection = await _dbConnectionFactory.CreateOpenConnectionAsync();

        var result = await dbConnection.QuerySingleAsync<string>(
            sql,
            new
            {
                Id = volunteerId,
                Phone = phone
            },
            commandTimeout: _options.QueryTimeout);

        if (string.IsNullOrWhiteSpace(result) == false)
        {
            _logger.LogWarning("Volunteer with phone {phone} already exists!", result);

            return UnitResult.Fail(Error.ValueIsAlreadyExist("Phone"));
        }
       
        _logger.LogInformation("Volunteer {phone} is unique!", result);

        return UnitResult.Ok();
    }

    public async Task<Result<VolunteerDto>> GetByIdAsync(
        Guid volunteerId,
        CancellationToken cancelToken = default)
    {
        var sql = $@"
            -- Constructing a single table where volunteer data is combined with their pets.
            -- The PetDtos column is stored as JSONB, containing an array of pet objects.
            SELECT 
                v.{VolunteersTable.Id} AS Id, 
                v.{VolunteersTable.UserId} AS UserId,
                v.{VolunteersTable.LastName} AS LastName, 
                v.{VolunteersTable.FirstName} AS FirstName, 
                v.{VolunteersTable.Phone} AS Phone,
                v.{VolunteersTable.Rating} AS Rating,
                v.{VolunteersTable.Requisites} AS RequisitesDtos,
                COALESCE(
                    jsonb_agg(
                        jsonb_build_object(
                            'PetId', p.{PetsTable.Id},           -- Pet ID
                            'PetName', p.{PetsTable.Name},       -- Pet name
                            'MainPhoto', p.{PetsTable.Images}->0->>'Name', -- Main photo (first image in the JSON array)
                            'StatusForHelp', p.{PetsTable.HelpStatus}, -- Help status
                            'BreedName', b.{BreedsTable.Name},   -- Breed name
                            'SpeciesName', s.{SpeciesTable.Name} -- Species name
                        )
                    ) FILTER (WHERE p.{PetsTable.Id} IS NOT NULL), '[]'::jsonb  -- If no pets exist, return an empty JSON array
                ) AS PetDtos
            FROM {VolunteersTable.TableFullName} v
            LEFT JOIN {PetsTable.TableFullName} p ON p.{PetsTable.VolunteerId} = v.{VolunteersTable.Id}
            LEFT JOIN {SpeciesTable.TableFullName} s ON s.{SpeciesTable.Id} = p.{PetsTable.PetTypeSpeciesId}
            LEFT JOIN {BreedsTable.TableFullName} b ON b.{BreedsTable.Id} = p.{PetsTable.PetTypeBreedId}
            WHERE v.{VolunteersTable.Id} = @Id
            GROUP BY v.{VolunteersTable.Id}  -- Group by volunteer to aggregate pets into a JSONB array
            LIMIT 1";

        _logger.LogInformation("Executing(GetByIdAsync for volunteerDTO) SQL Query: " +
            "{sql} with Parameters: {volunteerId}",
            sql, volunteerId);

        await using var dbConnection = await _dbConnectionFactory.CreateOpenConnectionAsync();

        var volunteer = await dbConnection.QuerySingleOrDefaultAsync<VolunteerDto>(
            sql,
            new { Id = volunteerId });

        if (volunteer == null)
        {
            _logger.LogWarning("Volunteer with id: {Id} not found!", volunteerId);
            return Result.Fail(Error.NotFound($"Volunteer with id:{volunteerId}"));
        }
        _logger.LogInformation("Get volunteer with id: {Id} successful!", volunteerId);

        return Result.Ok(volunteer);
    }

    public async Task<GetVolunteersResponse> GetVolunteers(
     GetVolunteersQuery query,
     CancellationToken cancelToken = default)
    {
        using var dbConnection = _dbConnectionFactory.CreateConnection();

        var orderBy = query.orderBy switch
        {
            "full_name" => "v.last_name, v.first_name",
            "rating" => "v.rating",
            _ => "v.Id"
        };
        var orderDirection = query.orderDirection switch
        {
            "asc" => "asc",
            _ => "desc"
        };

        var totalVolunteersCount = await dbConnection.ExecuteScalarAsync<int>(
            $"SELECT COUNT(1) FROM {VolunteersTable.TableFullName}" +
            $" WHERE {VolunteersTable.IsDeleted} = FALSE",
            commandTimeout: _options.QueryTimeout);

        var totalPages = (int)Math.Ceiling((double)totalVolunteersCount / query.pageSize);

        var pageNumber = query.pageNumber;
        if (pageNumber > totalPages || pageNumber <= 0)
        {
            _logger.LogWarning("Page number {pageNumber} exceeds total pages {totalPages}. " +
                "Returning empty result.", pageNumber, totalPages);
            return new GetVolunteersResponse(totalVolunteersCount, []);
        }

        var sql = $@"
            SELECT 
            v.{VolunteersTable.Id} AS Id, 
            v.{VolunteersTable.UserId} AS UserId,
            CONCAT(v.{VolunteersTable.LastName}, ' ', v.{VolunteersTable.FirstName}) AS FullName, 
            v.{VolunteersTable.Phone} AS Phone, 
            v.{VolunteersTable.Rating} AS Rating,
            v.{VolunteersTable.Requisites} AS RequisitesDtos
            FROM {VolunteersTable.TableFullName} v
            LEFT JOIN {UsersTable.TableFullName} u ON u.{UsersTable.Id} = v.{VolunteersTable.Id}
            WHERE v.{VolunteersTable.IsDeleted} = FALSE
            ORDER BY {orderBy} {orderDirection}
            LIMIT @Limit OFFSET @Offset";

        var offset = (query.pageNumber - 1) * query.pageSize;

        var limit = query.pageSize;

        _logger.LogInformation("Executing(GetVolunteers) SQL Query: {sql} with Parameters: {limit}, {offset}",
            sql, limit, offset);

        var volunteers = await dbConnection.QueryAsync<VolunteerMainInfoDto>(
                sql,
                new { Limit = limit, Offset = offset },
                commandTimeout: _options.QueryTimeout);

        var volunteersList = volunteers.ToList();

        _logger.LogInformation("GetVolunteers successful! Total count: {totalCount}", totalVolunteersCount);

        return new GetVolunteersResponse(totalVolunteersCount, volunteersList);
    }

    public async Task<Result<GetPetsResponse>> GetPetPagedList(
       PetsFilter? filter,
       int pageNumber,
       int pageSize,
       CancellationToken cancelToken = default)
    {
        var parameters = new DynamicParameters();

        var countBuilder = new StringBuilder($@"
            SELECT COUNT(*)
            FROM {PetsTable.TableFullName} p ");

        var sqlBuilder = new StringBuilder($@"
            SELECT 
            p.{PetsTable.Id} AS Id,
            p.{PetsTable.Name} AS PetName,
            p.{PetsTable.Color} AS Color,
            p.{PetsTable.Images}->0->>'Name' AS MainPhoto,
            p.{PetsTable.HelpStatus} AS StatusForHelp,
            DATE_PART('year', AGE(p.{PetsTable.DateOfBirth})) * 12 +
            DATE_PART('month', AGE(p.{PetsTable.DateOfBirth})) AS AgeInMonths,
            CONCAT(p.{PetsTable.AddressRegion}, ',',p.{PetsTable.AddressCity}, ',',p.{PetsTable.AddressStreet}) AS Address,
            v.{VolunteersTable.Id} AS VolunteerId,
            s.{SpeciesTable.Name} AS SpeciesName,
            b.{BreedsTable.Name} AS BreedName,
            CONCAT(v.{VolunteersTable.LastName}, ' ', v.{VolunteersTable.FirstName}) AS VolunteerFullName  
            FROM {PetsTable.TableFullName} p
            LEFT JOIN {VolunteersTable.TableFullName} v ON v.{VolunteersTable.Id} = p.{PetsTable.VolunteerId}
            LEFT JOIN {SpeciesTable.TableFullName} s ON s.{SpeciesTable.Id} = p.{PetsTable.PetTypeSpeciesId}
            LEFT JOIN {BreedsTable.TableFullName} b ON b.{BreedsTable.Id} = p.{PetsTable.PetTypeBreedId}
            WHERE p.{PetsTable.IsDeleted} = FALSE");

        if (filter != null)
        {
            countBuilder.AppendJoinsForGetPets(filter);
            countBuilder.AppendFiltersForGetPets(parameters, filter);

            sqlBuilder.AppendFiltersForGetPets(parameters, filter);
            sqlBuilder.AppendOrderByForGetPets(filter.GetOrderBies());
        }
        sqlBuilder.AppendPagination(pageNumber, pageSize, parameters);

        string sqlQuery = sqlBuilder.ToString();
        string countQuery = countBuilder.ToString();

        _logger.LogInformation("Executing(GetPets) SQL Query: {sql} with Parameters:", sqlQuery);
        _logger.LogInformation("Executing(GetPets) COUNT Query: {count} with Parameters:", countQuery);
        _logger.LogInformation("Parameters: {@Parameters}", parameters.ToDictionary());

        await using var connection1 = await _dbConnectionFactory.CreateOpenConnectionAsync();
        await using var connection2 = await _dbConnectionFactory.CreateOpenConnectionAsync();

        var petsTask = connection1.QueryAsync<PetWithVolunteerDto>(sqlQuery, parameters);
        var totalCountTask = connection2.ExecuteScalarAsync<int>(countQuery, parameters);

        await Task.WhenAll(petsTask, totalCountTask);

        var pets = await petsTask;
        var totalCount = await totalCountTask;

        return Result.Ok<GetPetsResponse>(new(totalCount, pets));
    }  
}
