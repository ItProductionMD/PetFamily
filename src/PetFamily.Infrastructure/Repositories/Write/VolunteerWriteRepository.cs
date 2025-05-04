using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PetFamily.Application.IRepositories;
using PetFamily.Domain.DomainError;
using PetFamily.Domain.PetManagment.Entities;
using PetFamily.Domain.PetManagment.Root;
using PetFamily.Domain.Results;
using PetFamily.Domain.Shared.ValueObjects;
using PetFamily.Infrastructure.Contexts;
using Polly;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.Data.SqlClient;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading;
using PetFamily.Domain.Shared.Validations;
using Npgsql;
using PetFamily.Application.Commands.SharedCommands;

namespace PetFamily.Infrastructure.Repositories.Write;

public class VolunteerWriteRepository(
    ILogger<VolunteerWriteRepository> logger,
    WriteDbContext context) : IVolunteerWriteRepository
{
    private readonly ILogger<VolunteerWriteRepository> _logger = logger;
    private readonly WriteDbContext _context = context;

    public async Task<Result<Guid>> AddAsync(
        Volunteer volunteer,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.Volunteers.AddAsync(volunteer, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Volunteer with Id:{} was updated successfull!", volunteer.Id);
            return Result.Ok(volunteer.Id);
        }
        catch (DbUpdateException ex)
            when (ex.InnerException is PostgresException pgEx && pgEx.SqlState == "23505")
        {
            string constraintName = pgEx.ConstraintName?.ToLower() ?? string.Empty;

            string[] uniqueFields = Volunteer.GetUniqueFields();

            var validationErrors = new List<ValidationError>();

            foreach (var field in uniqueFields)
            {
                if (constraintName.Contains(field))
                {
                    validationErrors.AddRange(
                        Error.ValueIsAlreadyExist(field).ValidationErrors);

                    _logger.LogWarning("Add volunteer with id:{Id} constraint error!" +
                        "{Value} already exist!", volunteer.Id, field);
                }
            }
            if (validationErrors.Count == 0)
            {
                _logger.LogError("Add volunteer with id:'{Id}' constraint error, but " +
                    "unique fields doesn't contain the constraint name:{contsraint}!",
                    volunteer.Id, constraintName);

                return UnitResult.Fail(Error.ValueIsAlreadyExist("Unknown"));
            }

            return UnitResult.Fail(Error.ValuesAreAlreadyExist(validationErrors));
        }
        catch (Exception ex)
        {
            _logger.LogCritical("Add volunteer with id:{Id} unexpected exception!Exception:{Message}",
                volunteer.Id, ex.Message);

            return UnitResult.Fail(Error.InternalServerError(
                $"Add volunteer unexpected error!"));
        }
    }

    public async Task<Result<Volunteer>> GetByIdAsync(Guid id, CancellationToken cancelToken)
    {
        var volunteer = await _context.Volunteers
            .Include(v => v.Pets)
            .FirstOrDefaultAsync(v => v.Id == id, cancelToken);

        if (volunteer == null)
        {
            _logger.LogError("Volunteer with id:{id} not found", id);
            return Result.Fail(Error.NotFound($"Volunteer with id:{id}"));
        }
        _logger.LogInformation("Volunteer with id:{id} found", id);
        return Result.Ok(volunteer);
    }

    public async Task<UnitResult> Save(Volunteer volunteer, CancellationToken cancellToken)
    {
        try
        {
            await _context.SaveChangesAsync(cancellToken);

            _logger.LogInformation("Volunteer with Id: {Id} updated succesfull!", volunteer.Id);

            return UnitResult.Ok();
        }
        catch (DbUpdateException ex)
           when (ex.InnerException is PostgresException pgEx && pgEx.SqlState == "23505")
        {
            string constraintName = pgEx.ConstraintName?.ToLower() ?? string.Empty;

            string[] uniqueFields = Volunteer.GetUniqueFields();

            var validationErrors = new List<ValidationError>();

            foreach (var field in uniqueFields)
            {
                if (constraintName.Contains(field))
                {
                    validationErrors.AddRange(
                        Error.ValueIsAlreadyExist(field).ValidationErrors);

                    _logger.LogWarning("Add volunteer with id:{Id} constraint error!" +
                        "{Value} already exist!", volunteer.Id, field);
                }
            }
            if (validationErrors.Count == 0)
            {
                _logger.LogError("Add volunteer with id:'{Id}' constraint error, but " +
                    "unique fields doesn't contain the constraint name:{contsraint}!",
                    volunteer.Id, constraintName);

                return UnitResult.Fail(Error.ValueIsAlreadyExist("Unknown"));
            }

            return UnitResult.Fail(Error.ValuesAreAlreadyExist(validationErrors));
        }
        catch (Exception ex)
        {
            _logger.LogCritical("Save volunteer with id:{Id}unexpected error!Error:{Message}",
                volunteer.Id, ex.Message);
            return UnitResult.Fail(Error.InternalServerError($"Save volunteer unexpected error!"));
        }
    }
    public async Task Delete(
        Volunteer volunteer,
        CancellationToken cancellToken = default)
    {
        _context.Volunteers.Remove(volunteer);
        await _context.SaveChangesAsync(cancellToken);
    }

    public async Task<UnitResult> UpdateSocialNetworks(
        Guid volunteerId,
        List<SocialNetworkInfo> socialNetworks,
        CancellationToken cancellation = default)
    {
        var socialNetworksValue = await _context.Volunteers
             .Where(v => v.Id == volunteerId)
             .Select(v => v.SocialNetworks)
             .FirstOrDefaultAsync(cancellation);

        if (socialNetworksValue == null)
            return Result.Fail(Error.NotFound("Volunteer"));

        if (socialNetworksValue == socialNetworks)
            return UnitResult.Ok();

        await _context.SaveChangesAsync(cancellation);

        return UnitResult.Ok();
    }

    public async Task SaveWithRetry(Volunteer volunteer, CancellationToken cancelToken = default)
    {
        var retryPolicy = Policy.Handle<Exception>().WaitAndRetryAsync(
               3, retryAttempt => TimeSpan.FromMilliseconds(200 * Math.Pow(2, retryAttempt)),
               onRetry: (exception, timeSpan, retryCount, context) =>
               {
                   _logger.LogWarning($"Attempt {retryCount} failed: {exception.Message}, " +
                       $"retrying in {timeSpan.TotalSeconds}s.");
               });

        await retryPolicy.ExecuteAsync(async () =>
        {
            await Save(volunteer, cancelToken);
        });
    }
}