using PetFamily.SharedApplication.Exceptions;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.ValueObjects;
using static PetFamily.SharedKernel.Validations.ValidationExtensions;

namespace Volunteers.Application.Commands.PetManagement.ChangeMainPetImage;


public static class ChangeMainPetImageValidation
{
    public static void Validate(this ChangePetMainImageCommand cmd)
    {
        var result = UnitResult.FromValidationResults(
            () => ValidateIfGuidIsNotEpmty(cmd.VolunteerId, "VolunteerId"),
            () => ValidateIfGuidIsNotEpmty(cmd.PetId, "PetId"),
            () => Image.Validate(cmd.imageName));

        if (result.IsFailure)
            throw new ValidationException(result.Error);
    }
}
