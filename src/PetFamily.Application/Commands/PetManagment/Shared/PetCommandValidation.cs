using PetFamily.Application.Commands.PetManagment.DeletePet;
using PetFamily.Domain.Results;
using static PetFamily.Domain.Shared.Validations.ValidationExtensions;

namespace PetFamily.Application.Commands.PetManagment.Shared;

public static class PetCommandValidation
{
    public static UnitResult Validate(PetCommand command)
    {
        return UnitResult.ValidateCollection(

            () => ValidateIfGuidIsNotEpmty(command.VolunteerId, "VolunteerId"),

            () => ValidateIfGuidIsNotEpmty(command.PetId, "PetId"));
    }
}
