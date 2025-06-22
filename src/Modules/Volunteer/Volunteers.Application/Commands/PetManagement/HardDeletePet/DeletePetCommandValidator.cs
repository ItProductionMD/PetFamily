using PetFamily.SharedKernel.Results;
using static PetFamily.SharedKernel.Validations.ValidationExtensions;

namespace Volunteers.Application.Commands.PetManagement.DeletePet;

public static class DeletePetCommandValidator
{
    public static UnitResult Validate(SoftDeletePetCommand command)
    {
        return UnitResult.FromValidationResults(

            () => ValidateIfGuidIsNotEpmty(command.VolunteerId, "VolunteerId"),

            () => ValidateIfGuidIsNotEpmty(command.PetId, "PetId"));
    }
    public static UnitResult Validate(HardDeletePetCommand command)
    {
        return UnitResult.FromValidationResults(

            () => ValidateIfGuidIsNotEpmty(command.VolunteerId, "VolunteerId"),

            () => ValidateIfGuidIsNotEpmty(command.PetId, "PetId"));
    }
}
