using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.ValueObjects;
using static PetFamily.SharedKernel.Validations.ValidationExtensions;

namespace Volunteers.Application.Commands.PetManagement.ChangeMainPetImage;


public static class ChangeMainPetImageValidation
{
    public static UnitResult Validate(ChangePetMainImageCommand command)
    {
        return UnitResult.ValidateCollection(

            () => ValidateIfGuidIsNotEpmty(command.VolunteerId, "VolunteerId"),

            () => ValidateIfGuidIsNotEpmty(command.PetId, "PetId"),

            () => Image.Validate(command.imageName));
    }
}
