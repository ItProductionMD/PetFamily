using System.Data;
using Microsoft.Extensions.Configuration;
using System.Data.Common;
using PetFamily.Application.Dtos;
using Dapper;
using PetFamily.Application.IRepositories;
using PetFamily.Application.Queries.Volunteer.GetVolunteers;
using PetFamily.Infrastructure.Dapper.GeneratedTables;
using System.Text.RegularExpressions;
using Amazon.Runtime.Internal.Util;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using PetFamily.Domain.Results;
using PetFamily.Domain.DomainError;
using System.Collections.Generic;
using Bogus.DataSets;
using PetFamily.Domain.Shared.Validations;
using PetFamily.Domain.PetManagment.Root;
using Microsoft.Extensions.Options;
using PetFamily.Infrastructure.Dapper;
using PetFamily.Application.Queries.Pet.GetPets;
using System.Text;
using PetFamily.Infrastructure.Extensions;

namespace PetFamily.Infrastructure.Repositories.Read;

public class VolunteerReadRepositoryWithDapper(
    IDbConnection dbConnection,
    IOptions<DapperOptions> options,
    ILogger<VolunteerReadRepositoryWithDapper> logger) : IVolunteerReadRepository
{
    private readonly IDbConnection _dbConnection = dbConnection;
    private readonly DapperOptions _options = options.Value;
    private readonly ILogger<VolunteerReadRepositoryWithDapper> _logger = logger;


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
                FROM {Volunteers.Table}
                WHERE {Volunteers.PhoneRegionCode} = @PhoneRegionCode 
                AND {Volunteers.PhoneNumber} = @PhoneNumber 
                AND {Volunteers.Id} <> @Id) 
                THEN 'Phone' ELSE NULL END AS PhoneTaken,
            CASE WHEN EXISTS (
                SELECT 1 FROM volunteers WHERE email = @Email 
                AND {Volunteers.Id} <> @Id) 
                THEN 'Email' ELSE NULL END AS EmailTaken";

        _logger.LogInformation("Executing(CheckUniqueFields) SQL Query: {sql} with Parameters:" +
            " {volunteerId}, {phoneRegionCode}, {phoneNumber}, {email}",
            sql, volunteerId, phoneRegionCode, phoneNumber, email);

        var result = await _dbConnection.QuerySingleAsync<(string PhoneTaken, string EmailTaken)>(
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

            return UnitResult.Fail(Error.ValuesAreAlreadyExist(validationErrors));
        }
        _logger.LogInformation("Volunteer {phone} {email} is/are unique!",
            result.PhoneTaken ?? "", result.EmailTaken ?? "");

        return UnitResult.Ok();
    }

    /// <summary>
    /// Retrieves volunteer information by their ID, including a list of their pets.
    /// </summary>
    /// <param name="volunteerId">The volunteer's unique identifier.</param>
    /// <param name="cancellToken">Cancellation token (optional).</param>
    /// <returns>A <see cref="VolunteerDto"/> object containing volunteer details and their pets.</returns>
    /// <exception cref="KeyNotFoundException">
    /// Thrown when a volunteer with the specified ID is not found.
    /// </exception>
    public async Task<Result<VolunteerDto>> GetByIdAsync(
        Guid volunteerId,
        CancellationToken cancelToken = default)
    {
        var sql = $@"
            -- Constructing a single table where volunteer data is combined with their pets.
            -- The PetDtos column is stored as JSONB, containing an array of pet objects.
            SELECT 
                v.{Volunteers.Id} AS Id, 
                v.{Volunteers.LastName} AS lastName, 
                v.{Volunteers.FirstName} AS firstName, 
                v.{Volunteers.PhoneRegionCode} AS phoneRegionCode,
                v.{Volunteers.PhoneNumber} AS phoneNumber,
                v.{Volunteers.Email} AS Email,
                v.{Volunteers.SocialNetworks} AS socialNetworkDtos,
                v.{Volunteers.Requisites} AS requisitesDtos,
                COALESCE(
                    jsonb_agg(
                        jsonb_build_object(
                            'PetId', p.{Pets.Id},           -- Pet ID
                            'PetName', p.{Pets.Name},       -- Pet name
                            'MainPhoto', p.{Pets.Images}->0->>'Name', -- Main photo (first image in the JSON array)
                            'StatusForHelp', p.{Pets.HelpStatus}, -- Help status
                            'BreedName', b.{Breeds.Name},   -- Breed name
                            'SpeciesName', s.{Species.Name} -- Species name
                        )
                    ) FILTER (WHERE p.{Pets.Id} IS NOT NULL), '[]'::jsonb  -- If no pets exist, return an empty JSON array
                ) AS PetDtos
            FROM {Volunteers.Table} v
            LEFT JOIN {Pets.Table} p ON p.{Pets.VolunteerId} = v.{Volunteers.Id}
            LEFT JOIN {Species.Table} s ON s.{Species.Id} = p.{Pets.PetTypeSpeciesId}
            LEFT JOIN {Breeds.Table} b ON b.{Breeds.Id} = p.{Pets.PetTypeBreedId}
            WHERE v.{Volunteers.Id} = @Id
            GROUP BY v.{Volunteers.Id}  -- Group by volunteer to aggregate pets into a JSONB array
            LIMIT 1";

        _logger.LogInformation("Executing(GetByIdAsync for volunteerDTO) SQL Query: " +
            "{sql} with Parameters: {volunteerId}",
            sql, volunteerId);

        var volunteer = await _dbConnection.QuerySingleOrDefaultAsync<VolunteerDto>(
            sql,
            new { Id = volunteerId },
            commandTimeout: _options.QueryTimeout);

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

        var totalVolunteersCount = await _dbConnection.ExecuteScalarAsync<int>(
            "SELECT COUNT(1) FROM Volunteers WHERE is_deleted = FALSE",
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
            FROM {Volunteers.Table} v
            WHERE v.{Volunteers.IsDeleted} = FALSE
            ORDER BY {orderBy} {orderDirection}
            LIMIT @Limit OFFSET @Offset";

        var offset = (query.pageNumber - 1) * query.pageSize;

        var limit = query.pageSize;

        _logger.LogInformation("Executing(GetVolunteers) SQL Query: {sql} with Parameters: {limit}, {offset}",
            sql, limit, offset);

        var volunteers = await _dbConnection.QueryAsync<VolunteerMainInfoDto>(
                sql,
                new { Limit = limit, Offset = offset },
                commandTimeout: _options.QueryTimeout);

        var volunteersList = volunteers.ToList();

        _logger.LogInformation("GetVolunteers successful! Total count: {totalCount}", totalVolunteersCount);

        return new GetVolunteersResponse(totalVolunteersCount, volunteersList);
    }

    public async Task<Result<GetPetsResponse>> GetPets(
       PetsFilter? filter,
       int pageNumber,
       int pageSize,
       CancellationToken cancelToken = default)
    {
        var parameters = new DynamicParameters();

        var countBuilder = new StringBuilder($@"
            SELECT COUNT(*)
            FROM {Pets.Table} p ");

        var sqlBuilder = new StringBuilder($@"
            SELECT 
            p.{Pets.Id} AS Id,
            p.{Pets.Name} AS PetName,
            p.{Pets.Color} AS Color,
            p.{Pets.Images}->0->>'Name' AS MainPhoto,
            p.{Pets.HelpStatus} AS StatusForHelp,
            DATE_PART('year', AGE(p.{Pets.DateOfBirth})) * 12 +
            DATE_PART('month', AGE(p.{Pets.DateOfBirth})) AS AgeInMonths,
            CONCAT(p.{Pets.AddressRegion}, ',',p.{Pets.AddressCity}, ',',p.{Pets.AddressStreet}) AS Address,
            v.{Volunteers.Id} AS VolunteerId,
            s.{Species.Name} AS SpeciesName,
            b.{Breeds.Name} AS BreedName,
            CONCAT(v.{Volunteers.LastName}, ' ', v.{Volunteers.FirstName}) AS VolunteerFullName  
            FROM {Pets.Table} p
            LEFT JOIN {Volunteers.Table} v ON v.{Volunteers.Id} = p.{Pets.VolunteerId}
            LEFT JOIN {Species.Table} s ON s.{Species.Id} = p.{Pets.PetTypeSpeciesId}
            LEFT JOIN {Breeds.Table} b ON b.{Breeds.Id} = p.{Pets.PetTypeBreedId}
            WHERE p.{Pets.IsDeleted} = FALSE");

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

        var pets = await _dbConnection.QueryAsync<PetWithVolunteerDto>(sqlQuery, parameters);

        var totalCount = await _dbConnection.ExecuteScalarAsync<int>(countQuery, parameters);

        return Result.Ok<GetPetsResponse>(new(totalCount, pets));
    }
}
