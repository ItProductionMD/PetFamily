using Dapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PetFamily.Application.Abstractions;
using PetFamily.SharedInfrastructure.Shared.Dapper;
using PetFamily.SharedInfrastructure.Shared.Dapper.ScaffoldedClasses;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.Validations;
using System.Text;
using Volunteers.Application.IRepositories;
using Volunteers.Application.Queries.GetPets;
using Volunteers.Application.Queries.GetPets.ForFilter;
using Volunteers.Application.Queries.GetVolunteers;
using Volunteers.Application.ResponseDtos;
using Volunteers.Infrastructure.Extensions;
using Volunteers.Public.IContracts;

using static PetFamily.SharedInfrastructure.Shared.Extensions.DynamicParametersExtensions;


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
        string phoneRegionCode,
        string phoneNumber,
        string email,
        CancellationToken cancellationToken = default)
    {
        var sql = $@"
        SELECT 
            CASE WHEN EXISTS (SELECT 1 
                FROM {VolunteerTable.TableFullName}
                WHERE {VolunteerTable.PhoneRegionCode} = @PhoneRegionCode 
                AND {VolunteerTable.PhoneNumber} = @PhoneNumber 
                AND {VolunteerTable.Id} <> @Id) 
                THEN 'Phone' ELSE NULL END AS PhoneTaken,
            CASE WHEN EXISTS (
                SELECT 1 FROM {VolunteerTable.TableFullName} WHERE email = @Email 
                AND {VolunteerTable.Id} <> @Id) 
                THEN 'Email' ELSE NULL END AS EmailTaken";

        _logger.LogInformation("Executing(CheckUniqueFields) SQL Query: {sql} with Parameters:" +
            " {volunteerId}, {phoneRegionCode}, {phoneNumber}, {email}",
            sql, volunteerId, phoneRegionCode, phoneNumber, email);

        await using var dbConnection = await _dbConnectionFactory.CreateOpenConnectionAsync();

        var result = await dbConnection.QuerySingleAsync<(string PhoneTaken, string EmailTaken)>(
            sql,
            new
            {
                Id = volunteerId,
                PhoneRegionCode = phoneRegionCode,
                PhoneNumber = phoneNumber,
                Email = email
            },
            commandTimeout: _options.QueryTimeout);

        List<ValidationError> validationErrors = [];

        if (string.IsNullOrWhiteSpace(result.PhoneTaken) == false)
            validationErrors.Add(Error.ValueIsAlreadyExist("Phone").ValidationErrors.FirstOrDefault()!);

        if (string.IsNullOrWhiteSpace(result.EmailTaken) == false)
            validationErrors.Add(Error.ValueIsAlreadyExist("Email").ValidationErrors.FirstOrDefault()!);

        if (validationErrors.Count > 0)
        {
            _logger.LogWarning("Volunteer {phone} {email} is/are already busy!",
                result.PhoneTaken ?? "", result.EmailTaken ?? "");

            return Result.Fail(Error.ValuesAreAlreadyExist(validationErrors));
        }
        _logger.LogInformation("Volunteer {phone} {email} is/are unique!",
            result.PhoneTaken ?? "", result.EmailTaken ?? "");

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
                v.{VolunteerTable.Id} AS Id, 
                v.{VolunteerTable.LastName} AS lastName, 
                v.{VolunteerTable.FirstName} AS firstName, 
                v.{VolunteerTable.PhoneRegionCode} AS phoneRegionCode,
                v.{VolunteerTable.PhoneNumber} AS phoneNumber,
                v.{VolunteerTable.Email} AS Email,
                v.{VolunteerTable.SocialNetworks} AS socialNetworkDtos,
                v.{VolunteerTable.Requisites} AS requisitesDtos,
                COALESCE(
                    jsonb_agg(
                        jsonb_build_object(
                            'PetId', p.{PetTable.Id},           -- Pet ID
                            'PetName', p.{PetTable.Name},       -- Pet name
                            'MainPhoto', p.{PetTable.Images}->0->>'Name', -- Main photo (first image in the JSON array)
                            'StatusForHelp', p.{PetTable.HelpStatus}, -- Help status
                            'BreedName', b.{BreedTable.Name},   -- Breed name
                            'SpeciesName', s.{SpeciesTable.Name} -- Species name
                        )
                    ) FILTER (WHERE p.{PetTable.Id} IS NOT NULL), '[]'::jsonb  -- If no pets exist, return an empty JSON array
                ) AS PetDtos
            FROM {VolunteerTable.TableFullName} v
            LEFT JOIN {PetTable.TableFullName} p ON p.{PetTable.VolunteerId} = v.{VolunteerTable.Id}
            LEFT JOIN {SpeciesTable.TableFullName} s ON s.{SpeciesTable.Id} = p.{PetTable.PetTypeSpeciesId}
            LEFT JOIN {BreedTable.TableFullName} b ON b.{BreedTable.Id} = p.{PetTable.PetTypeBreedId}
            WHERE v.{VolunteerTable.Id} = @Id
            GROUP BY v.{VolunteerTable.Id}  -- Group by volunteer to aggregate pets into a JSONB array
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
            $"SELECT COUNT(1) FROM {VolunteerTable.TableFullName}" +
            $" WHERE {VolunteerTable.IsDeleted} = FALSE",
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
            SELECT v.Id, 
            CONCAT(v.last_name, ' ', v.first_name) AS FullName, 
            CONCAT(v.phone_region_code, '-', v.phone_number) AS Phone, 
            v.rating
            FROM {VolunteerTable.TableFullName} v
            WHERE v.{VolunteerTable.IsDeleted} = FALSE
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
            FROM {PetTable.TableFullName} p ");

        var sqlBuilder = new StringBuilder($@"
            SELECT 
            p.{PetTable.Id} AS Id,
            p.{PetTable.Name} AS PetName,
            p.{PetTable.Color} AS Color,
            p.{PetTable.Images}->0->>'Name' AS MainPhoto,
            p.{PetTable.HelpStatus} AS StatusForHelp,
            DATE_PART('year', AGE(p.{PetTable.DateOfBirth})) * 12 +
            DATE_PART('month', AGE(p.{PetTable.DateOfBirth})) AS AgeInMonths,
            CONCAT(p.{PetTable.AddressRegion}, ',',p.{PetTable.AddressCity}, ',',p.{PetTable.AddressStreet}) AS Address,
            v.{VolunteerTable.Id} AS VolunteerId,
            s.{SpeciesTable.Name} AS SpeciesName,
            b.{BreedTable.Name} AS BreedName,
            CONCAT(v.{VolunteerTable.LastName}, ' ', v.{VolunteerTable.FirstName}) AS VolunteerFullName  
            FROM {PetTable.TableFullName} p
            LEFT JOIN {VolunteerTable.TableFullName} v ON v.{VolunteerTable.Id} = p.{PetTable.VolunteerId}
            LEFT JOIN {SpeciesTable.TableFullName} s ON s.{SpeciesTable.Id} = p.{PetTable.PetTypeSpeciesId}
            LEFT JOIN {BreedTable.TableFullName} b ON b.{BreedTable.Id} = p.{PetTable.PetTypeBreedId}
            WHERE p.{PetTable.IsDeleted} = FALSE");

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
