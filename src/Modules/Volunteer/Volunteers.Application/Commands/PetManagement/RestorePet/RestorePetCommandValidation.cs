using PetFamily.SharedApplication.Exceptions;
using PetFamily.SharedKernel.Results;
using static PetFamily.SharedKernel.Validations.ValidationExtensions;

namespace Volunteers.Application.Commands.PetManagement.RestorePet;

public static class RestorePetCommandValidation
{
    public static void Validate(this RestorePetCommand command)
    {
        var result = UnitResult.FromValidationResults(

            () => ValidateIfGuidIsNotEpmty(command.VolunteerId, "VolunteerId"),

            () => ValidateIfGuidIsNotEpmty(command.PetId, "PetId"));

        if (result.IsFailure)
            throw new ValidationException(result.Error);
    }
}
