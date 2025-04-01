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
        var limit = query.pageSize + 1;

        _logger.LogInformation("Executing(GetVolunteers) SQL Query: {sql} with Parameters: {limit}, {offset}",
            sql, limit, offset);

        var volunteers = await _dbConnection.QueryAsync<VolunteerMainInfoDto>(
                sql,
                new { Limit = limit, Offset = offset },
                commandTimeout: _options.QueryTimeout);

        var volunteersList = volunteers.ToList();

        bool hasMoreRecords = volunteersList.Count > query.pageSize;

        var volunteersCount = (query.pageNumber - 1) * query.pageSize + volunteersList.Count;

        if (hasMoreRecords)
        {
            volunteersList.RemoveAt(volunteersList.Count - 1);

            var getVolunteerCountSql = "SELECT COUNT(1) FROM Volunteers";

            _logger.LogInformation("Executing(GetVolunteers) SQL Query:{sql}", getVolunteerCountSql);

            volunteersCount = await _dbConnection.ExecuteScalarAsync<int>(
                getVolunteerCountSql,
                commandTimeout:_options.QueryTimeout);
        }

        _logger.LogInformation("GetVolunteers successful! Total count: {totalCount}", volunteersCount);

        return new GetVolunteersResponse(volunteersCount, volunteersList);
    }
}
