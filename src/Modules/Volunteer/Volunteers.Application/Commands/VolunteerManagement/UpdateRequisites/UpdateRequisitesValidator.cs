using PetFamily.SharedApplication.Exceptions;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.ValueObjects;
using PetFamily.SharedKernel.ValueObjects.Ids;
using static PetFamily.SharedKernel.Validations.ValidationExtensions;

namespace Volunteers.Application.Commands.VolunteerManagement.UpdateRequisites;

public static class UpdateRequisitesValidator
{
    public static void Validate(UpdateRequisitesCommand cmd)
    {
        var validationResult = UnitResult.FromValidationResults(
            ()=> ValidateIfGuidIsNotEpmty(cmd.UserId,"UserId"),
            ()=> ValidateIfGuidIsNotEpmty(cmd.VolunteerId,"VolunteerId"),
            ()=> ValidateItems(cmd.RequisitesDtos, r => RequisitesInfo.Validate(r.Name, r.Description))
        );

        if (validationResult.IsFailure)
            throw new ValidationException(validationResult.Error);
    }
}
