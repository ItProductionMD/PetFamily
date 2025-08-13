using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.Validations;
using Polly;
using Volunteers.Application.IRepositories;
using Volunteers.Domain;
using Volunteers.Infrastructure.Contexts;

namespace Volunteers.Infrastructure.Repositories;

public class VolunteerWriteRepository(
    VolunteerWriteDbContext context,
    ILogger<VolunteerWriteRepository> logger) : IVolunteerWriteRepository
{
    public async Task<Result<Guid>> AddAndSaveAsync(
        Volunteer volunteer,
        CancellationToken ct = default)
    {
        try
        {
            await context.Volunteers.AddAsync(volunteer, ct);
            await context.SaveChangesAsync(ct);

            logger.LogInformation("Volunteer with Id:{} was updated successfull!", volunteer.Id);
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

                    logger.LogWarning("Add volunteer with id:{Id} constraint error!" +
                        "{Value} already exist!", volunteer.Id, field);
                }
            }
            if (validationErrors.Count == 0)
            {
                logger.LogError("Add volunteer with id:'{Id}' constraint error, but " +
                    "unique fields doesn't contain the constraint name:{contsraint}!",
                    volunteer.Id, constraintName);

                return UnitResult.Fail(Error.ValueIsAlreadyExist("Unknown"));
            }

            return UnitResult.Fail(Error.ValuesAreAlreadyExist(validationErrors));
        }
        catch (Exception ex)
        {
            logger.LogCritical("Add volunteer with id:{Id} unexpected exception!Exception:{Message}",
                volunteer.Id, ex.Message);

            return UnitResult.Fail(Error.InternalServerError(
                $"Add volunteer unexpected error!"));
        }
    }

    public async Task<Result<Volunteer>> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var volunteer = await context.Volunteers
            .Include(v => v.Pets)
            .FirstOrDefaultAsync(v => v.Id == id, ct);

        if (volunteer == null)
        {
            logger.LogError("Volunteer with id:{id} not found", id);
            return Result.Fail(Error.NotFound($"Volunteer with id:{id}"));
        }
        logger.LogInformation("Volunteer with id:{id} found", id);
        return Result.Ok(volunteer);
    }

    public async Task<UnitResult> SaveAsync(Volunteer volunteer, CancellationToken ct)
    {
        try
        {
            await context.SaveChangesAsync(ct);

            logger.LogInformation("Volunteer with Id: {Id} updated successful !", volunteer.Id);

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

                    logger.LogWarning("Add volunteer with id:{Id} constraint error!" +
                        "{Value} already exist!", volunteer.Id, field);
                }
            }
            if (validationErrors.Count == 0)
            {
                logger.LogError("Add volunteer with id:'{Id}' constraint error, but " +
                    "unique fields doesn't contain the constraint name:{contsraint}!",
                    volunteer.Id, constraintName);

                return UnitResult.Fail(Error.ValueIsAlreadyExist("Unknown"));
            }

            return UnitResult.Fail(Error.ValuesAreAlreadyExist(validationErrors));
        }
        catch (Exception ex)
        {
            logger.LogCritical("Save volunteer with id:{Id}unexpected error!Error:{Message}",
                volunteer.Id, ex.Message);
            return UnitResult.Fail(Error.InternalServerError($"Save volunteer unexpected error!"));
        }
    }

    public async Task Delete(
        Volunteer volunteer,
        CancellationToken ct = default)
    {
        context.Volunteers.Remove(volunteer);
        await context.SaveChangesAsync(ct);
    }


    public async Task SaveWithRetry(Volunteer volunteer, CancellationToken ct = default)
    {
        var retryPolicy = Policy.Handle<Exception>().WaitAndRetryAsync(
               3, retryAttempt => TimeSpan.FromMilliseconds(200 * Math.Pow(2, retryAttempt)),
               onRetry: (exception, timeSpan, retryCount, context) =>
               {
                   logger.LogWarning($"Attempt {retryCount} failed: {exception.Message}, " +
                       $"retrying in {timeSpan.TotalSeconds}s.");
               });

        await retryPolicy.ExecuteAsync(async () =>
        {
            await SaveAsync(volunteer, ct);
        });
    }

    public async Task<Result<Volunteer>> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
    {
        var volunteer = await context.Volunteers
            .Include(v => v.Pets)
            .FirstOrDefaultAsync(v => v.UserId.Value == userId, ct);

        if (volunteer == null)
        {
            logger.LogError("Volunteer with userId:{userId} not found", userId);
            return Result.Fail(Error.NotFound($"Volunteer with userId:{userId}"));
        }
        logger.LogInformation("Volunteer with userId:{userId} found", userId);

        return Result.Ok(volunteer);
    }
}