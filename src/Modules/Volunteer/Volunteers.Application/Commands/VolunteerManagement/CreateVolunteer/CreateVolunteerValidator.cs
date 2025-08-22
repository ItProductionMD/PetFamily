using PetFamily.SharedApplication.Exceptions;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.ValueObjects;
using static PetFamily.SharedKernel.Validations.ValidationConstants;
using static PetFamily.SharedKernel.Validations.ValidationExtensions;

namespace Volunteers.Application.Commands.VolunteerManagement.CreateVolunteer;

public static class CreateVolunteerValidator
{
    public static void Validate(this CreateVolunteerCommand cmd)
    {
        var result = UnitResult.FromValidationResults(

            () => FullName.Validate(cmd.FirstName, cmd.LastName),
            () => ValidateNonRequiredField(cmd.Description, "Description", MAX_LENGTH_LONG_TEXT),
            () => ValidateIntegerNumber(cmd.ExperienceYears, "Experience years", minValue: 0, maxValue: 100),
            () => ValidateItems(cmd.Requisites, r => RequisitesInfo.Validate(r.Name, r.Description)),
            () => ValidateIfGuidIsNotEpmty(cmd.AdminId, "AdminId"),
            () => ValidateIfGuidIsNotEpmty(cmd.UserId, "UserId")
        );

        if (result.IsFailure)
            throw new ValidationException(result.Error);
    }
}
