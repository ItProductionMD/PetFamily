using PetFamily.Domain.PetManagment.Enums;
using PetFamily.Domain.Results;
using static PetFamily.Domain.Shared.Validations.ValidationExtensions;

namespace PetFamily.Application.Commands.PetManagment.UpdatePetStatus;

public static class UpdatePetStatusValidator
{
    public static UnitResult Validate(UpdatePetStatusCommand command)
    {
        return UnitResult.ValidateCollection(

            () => ValidateIfGuidIsNotEpmty(command.VolunteerId, "Volunteer Id"),

            () => ValidateIfGuidIsNotEpmty(command.PetId, "Pet Id"),

            () => ValidateNumber(
                command.HelpStatus,
                "Pet HelpStatus",
                0,
                Enum.GetValues<HelpStatus>().Length));
    }
}
