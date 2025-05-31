using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.ValueObjects;
using static PetFamily.SharedKernel.Validations.ValidationConstants;
using static PetFamily.SharedKernel.Validations.ValidationExtensions;
using static PetFamily.SharedKernel.Validations.ValidationPatterns;

namespace Volunteers.Application.Commands.VolunteerManagement.CreateVolunteer;

public static class CreateVolunteerRequestValidatorCustom
{
    public static UnitResult Validate(CreateVolunteerCommand volunteer)
    {
        return UnitResult.ValidateCollection(

            () => Phone.Validate(volunteer.PhoneNumber, volunteer.PhoneRegionCode),

            () => FullName.Validate(volunteer.FirstName, volunteer.LastName),

            () => ValidateRequiredField(
                volunteer.Email, "Email", MAX_LENGTH_SHORT_TEXT, EMAIL_PATTERN),

            () => ValidateNonRequiredField(
                volunteer.Description, "Description", MAX_LENGTH_LONG_TEXT),

            () => ValidateIntegerNumber(
                volunteer.ExperienceYears, "Experience years", minValue: 0, maxValue: 100),

            () => ValidateItems(
                volunteer.SocialNetworksList, s => SocialNetworkInfo.Validate(s.Name, s.Url)),

            () => ValidateItems(
                volunteer.Requisites, r => RequisitesInfo.Validate(r.Name, r.Description)));
    }
}
