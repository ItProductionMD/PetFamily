using Microsoft.Extensions.Logging;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.DomainResult;
using PetFamily.Domain.VolunteerAggregates.Root;

namespace PetFamily.Application.Volunteers;

public static class VolunteerValidationExtensions
{
    /// <summary>
    /// Verify that the email and phone number are unique
    /// </summary>
    /// <param name="volunteer"></param>
    /// <param name="volunteerRepository"></param>
    /// <returns>Task<Result></returns>
    public static async Task<Result> ValidateUniqueEmailAndPhone<T>(
        Volunteer volunteer,
        IVolunteerRepository volunteerRepository,
        ILogger<T> logger,
         CancellationToken cancellationToken)
    {
        //--------------------------Get Volunteers with souch email or phone----------------------//
        var getVolunteers = await volunteerRepository
            .GetByEmailOrPhone(volunteer.Email, volunteer.PhoneNumber, cancellationToken);

        if (getVolunteers.IsFailure)
            return Result.Success();

        var existingVolunteers = getVolunteers.Data;

        List<Error> errors = [];
        //-------------------------------Creating Error List--------------------------------------//
        foreach (var v in existingVolunteers)
        {
            if (v.Id == volunteer.Id)
                break;

            if (v.Email == volunteer.Email)
                errors.Add(Error.CreateErrorValueIsBusy("Email"));

            if (v.PhoneNumber == volunteer.PhoneNumber)
                errors.Add(Error.CreateErrorValueIsBusy("Phone"));
        }
        if (errors.Count == 0)
            return Result.Success();

        logger.LogError("Validate volunteer unique phone and unique email failure!{errors}", errors);

        return Result.Failure(errors!);
    }
}
