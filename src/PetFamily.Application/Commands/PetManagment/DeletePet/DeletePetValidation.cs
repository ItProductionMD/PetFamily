using static PetFamily.Domain.Shared.Validations.ValidationExtensions;
using PetFamily.Domain.Results;

namespace PetFamily.Application.Commands.PetManagment.DeletePet;

public static class DeletePetValidation
{
    public static UnitResult Validate(SoftDeletePetCommand command)
    {
        return UnitResult.ValidateCollection(

            () => ValidateIfGuidIsNotEpmty(command.VolunteerId,"VolunteerId"),

            () => ValidateIfGuidIsNotEpmty(command.PetId,"PetId"));
    }
    public static UnitResult Validate(HardDeletePetCommand command)
    {
        return UnitResult.ValidateCollection(

            () => ValidateIfGuidIsNotEpmty(command.VolunteerId, "VolunteerId"),

            () => ValidateIfGuidIsNotEpmty(command.PetId, "PetId"));
    }
}
