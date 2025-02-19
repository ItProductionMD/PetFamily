using Microsoft.Extensions.Logging;
using PetFamily.Domain.DomainError;
using PetFamily.Domain.Results;
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
    public static async Task<UnitResult> ValidateUniqueEmailAndPhone<T>(
        Volunteer volunteer,
        IVolunteerRepository volunteerRepository,
        ILogger<T> logger,
         CancellationToken cancellationToken)
    {
        //--------------------------Get Volunteers with souch email or phone----------------------//
        var getVolunteers = await volunteerRepository
            .GetByEmailOrPhone(volunteer.Email, volunteer.PhoneNumber, cancellationToken);

        if (getVolunteers.IsFailure)
            return UnitResult.Ok();

        List<Volunteer> existingVolunteers = getVolunteers.Data!;
        List<Error> errors = [];
        //-------------------------------Creating Error List--------------------------------------//
        foreach (var v in existingVolunteers)
        {
            if (v.Id == volunteer.Id)
                break;

            if (v.Email == volunteer.Email)
                errors.Add(Error.ValueIsBusy("Email"));

            if (v.PhoneNumber == volunteer.PhoneNumber)
                errors.Add(Error.ValueIsBusy("Phone"));
        }
        if (errors.Count == 0)
            return UnitResult.Ok();

        return UnitResult.Fail(errors!);
    }
}
