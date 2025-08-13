using PetFamily.SharedKernel.Results;
using static PetFamily.SharedKernel.Validations.ValueObjectValidations;
using static PetFamily.SharedKernel.Validations.ValidationExtensions;
using PetFamily.SharedApplication.Exceptions;

namespace Volunteers.Application.Commands.VolunteerManagement.UpdateVolunteerPhone;

public static class UpdateVolunteerPhoneValidator
{
    public static void Validate(this UpdateVolunteerPhoneCommand cmd)
    {
        var validationResult = UnitResult.FromValidationResults(
            ()=> ValidateIfGuidIsNotEpmty(cmd.UserId, "UserId"),
            ()=> ValidateRequiredPhone(cmd.PhoneRegionCode, cmd.PhoneNumber)
        );

        if (validationResult.IsFailure)
            throw new ValidationException(validationResult.Error);
        
    }
}
