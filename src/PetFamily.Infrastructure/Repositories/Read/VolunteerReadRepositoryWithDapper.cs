﻿using System.Data;
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

namespace PetFamily.Infrastructure.Repositories.Read;

public class VolunteerReadRepositoryWithDapper(
    IDbConnection dbConnection,
    ILogger<VolunteerReadRepositoryWithDapper> logger) : IVolunteerReadRepository
{
    private readonly IDbConnection _dbConnection = dbConnection;
    private readonly ILogger<VolunteerReadRepositoryWithDapper> _logger = logger;

    public async Task<UnitResult> CheckUniqueFields(
        Guid volunteerId,
        string phoneRegionCode,
        string phoneNumber,
        string email,
        CancellationToken cancelToken = default)
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

        var result = await _dbConnection.QuerySingleAsync<(string PhoneTaken, string EmailTaken)>(
            sql,
            new
            {
                Id = volunteerId,
                PhoneRegionCode = phoneRegionCode,
                PhoneNumber = phoneNumber,
                Email = email
            });

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

        _logger.LogInformation("Executing SQL Query: {sql} with Parameters: {volunteerId}",
            sql, volunteerId);

        var volunteer = await _dbConnection.QuerySingleOrDefaultAsync<VolunteerDto>(
            sql,
            new { Id = volunteerId });

        if (volunteer == null)
        {
            _logger.LogWarning("Volunteer with id: {Id} not found!", volunteerId);
            return Result.Fail(Error.NotFound($"Volunteer with id:{volunteerId}"));
        }
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

        var offset = (query.pageNumber - 1) * query.pageSize;
        var limit = query.pageSize + 1;

        var volunteersList = await _dbConnection.QueryAsync<VolunteerMainInfoDto>(
            $@"
                SELECT v.Id, 
                CONCAT(v.last_name, ' ', v.first_name) AS FullName, 
                CONCAT(v.phone_region_code, '-', v.phone_number) AS Phone, 
                v.rating
                FROM {Volunteers.Table} v
                WHERE v.{Volunteers.IsDeleted} = FALSEs
                ORDER BY {orderBy} {orderDirection}
                LIMIT @Limit OFFSET @Offset",
                new { Limit = limit, Offset = offset });

        var volunteersListAsList = volunteersList.ToList();
        bool hasMoreRecords = volunteersListAsList.Count > query.pageSize;

        if (hasMoreRecords)
            volunteersListAsList.RemoveAt(volunteersListAsList.Count - 1);

        var totalCount = hasMoreRecords
            ? await _dbConnection.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM Volunteers")
            : (query.pageNumber - 1) * query.pageSize + volunteersListAsList.Count;

        return new GetVolunteersResponse(totalCount, volunteersListAsList);
    }
}
