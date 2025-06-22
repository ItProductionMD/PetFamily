using PetFamily.SharedKernel.Results;
using Volunteers.Domain.Enums;
using static PetFamily.SharedKernel.Validations.ValidationExtensions;

namespace Volunteers.Application.Commands.PetManagement.UpdatePetStatus;

public static class UpdatePetStatusCommandValidator
{
    public static UnitResult Validate(UpdatePetStatusCommand command)
    {
        return UnitResult.FromValidationResults(

            () => ValidateIfGuidIsNotEpmty(command.VolunteerId, "Volunteer Id"),

            () => ValidateIfGuidIsNotEpmty(command.PetId, "Pet Id"),

            () => ValidateNumber(
                command.HelpStatus,
                "Pet HelpStatus",
                0,
                Enum.GetValues<HelpStatus>().Length));
    }
}
