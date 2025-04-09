using PetFamily.Application.Commands.PetManagment.DeletePet;
using PetFamily.Domain.Results;
using PetFamily.Domain.Shared.ValueObjects;
using static PetFamily.Domain.Shared.Validations.ValidationExtensions;

namespace PetFamily.Application.Commands.PetManagment.ChangeMainPetImage;

public static class ChangeMainPetImageValidation
{
    public static UnitResult Validate(ChangeMainPetImageCommand command)
    {
        return UnitResult.ValidateCollection(

            () => ValidateIfGuidIsNotEpmty(command.VolunteerId, "VolunteerId"),

            () => ValidateIfGuidIsNotEpmty(command.PetId, "PetId"),

            () => Image.Validate(command.imageName));
    }
}
