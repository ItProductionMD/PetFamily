using PetFamily.SharedApplication.Exceptions;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.ValueObjects;
using static PetFamily.SharedKernel.Validations.ValidationConstants;
using static PetFamily.SharedKernel.Validations.ValidationExtensions;
using static PetFamily.SharedKernel.Validations.ValidationPatterns;

namespace Volunteers.Application.Commands.VolunteerManagement.CreateVolunteer;

public static class CreateVolunteerValidator
{
    public static void Validate(CreateVolunteerCommand volunteer)
    {
        var result =  UnitResult.FromValidationResults(

            () => FullName.Validate(volunteer.FirstName, volunteer.LastName),

            () => ValidateNonRequiredField(
                volunteer.Description, "Description", MAX_LENGTH_LONG_TEXT),

            () => ValidateIntegerNumber(
                volunteer.ExperienceYears, "Experience years", minValue: 0, maxValue: 100),

            () => ValidateItems(
                volunteer.Requisites, r => RequisitesInfo.Validate(r.Name, r.Description)));

        if (result.IsFailure)
            throw new ValidationException(result.Error);
    }
}
