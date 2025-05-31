using PetFamily.SharedKernel.Results;
using static PetFamily.SharedKernel.Validations.ValidationExtensions;

namespace Volunteers.Application.Commands.PetManagement.RestorePet;

public static class RestorePetCommandValidation
{
    public static UnitResult Validate(RestorePetCommand command)
    {
        return UnitResult.ValidateCollection(

            () => ValidateIfGuidIsNotEpmty(command.VolunteerId, "VolunteerId"),

            () => ValidateIfGuidIsNotEpmty(command.PetId, "PetId"));
    }
}
