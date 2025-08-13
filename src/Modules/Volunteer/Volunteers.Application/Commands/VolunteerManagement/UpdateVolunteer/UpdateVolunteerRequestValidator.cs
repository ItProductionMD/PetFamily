using PetFamily.SharedKernel.Results;
using static PetFamily.SharedKernel.Validations.ValueObjectValidations;
using static PetFamily.SharedKernel.Validations.ValidationExtensions;
using static PetFamily.SharedKernel.Validations.ValidationConstants;
using PetFamily.SharedApplication.Exceptions;


namespace Volunteers.Application.Commands.VolunteerManagement.UpdateVolunteer;

public static class UpdateVolunteerRequestValidator
{
    public static void Validate(this UpdateVolunteerCommand cmd)
    {
        var validationResult = UnitResult.FromValidationResults(
            () => ValidateFullName(cmd.FirstName, cmd.LastName),
            () => ValidateIfGuidIsNotEpmty(cmd.UserId, "UserId"),
            () => ValidateIfGuidIsNotEpmty(cmd.VolunteerId, "VolunteerId"),
            () => ValidateNonRequiredField(cmd.Description, "Description", MAX_LENGTH_LONG_TEXT),
            () => ValidateIntegerNumber(cmd.ExperienceYears, "ExperienceYears", 0, 100)
        );

        if (validationResult.IsFailure)
            throw new ValidationException(validationResult.Error);
    }
}
