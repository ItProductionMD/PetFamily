using PetFamily.SharedApplication.Exceptions;
using PetFamily.SharedKernel.Results;
using static PetFamily.SharedKernel.Validations.ValidationExtensions;

namespace Volunteers.Application.Commands.PetManagement.DeletePet;

public static class DeletePetCommandValidator
{
    public static void Validate(this SoftDeletePetCommand command)
    {
        var result = UnitResult.FromValidationResults(

            () => ValidateIfGuidIsNotEpmty(command.VolunteerId, "VolunteerId"),

            () => ValidateIfGuidIsNotEpmty(command.PetId, "PetId"));

        if (result.IsFailure)
            throw new ValidationException(result.Error);
    }
    public static void Validate(this HardDeletePetCommand command)
    {
        var result = UnitResult.FromValidationResults(

            () => ValidateIfGuidIsNotEpmty(command.VolunteerId, "VolunteerId"),

            () => ValidateIfGuidIsNotEpmty(command.PetId, "PetId"));

        if (result.IsFailure)
            throw new ValidationException(result.Error);
    }
}
