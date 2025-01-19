using PetFamily.Application.Volunteers.CreateVolunteer;
using PetFamily.Domain.Shared.DomainResult;
using PetFamily.Domain.Shared.ValueObjects;
using static PetFamily.Domain.Shared.Validations.ValidationExtensions;
using static PetFamily.Domain.Shared.Validations.ValidationConstants;
using static PetFamily.Domain.Shared.Validations.ValidationPatterns;

namespace PetFamily.Application.Validations;

public static class VolunteerRequestValidator
{
    public static Result Validate(CreateVolunteerRequest volunteer)
    {
        return Result.ValidateCollection(

            () => Phone.Validate(volunteer.PhoneNumber, volunteer.PhoneRegionCode),

            () => FullName.Validate(volunteer.FirstName, volunteer.LastName),

            () => ValidateRequiredField(volunteer.Email, "Email", MAX_LENGTH_SHORT_TEXT, EMAIL_PATTERN),

            () => ValidateNonRequiredField(volunteer.Description, "Description", MAX_LENGTH_LONG_TEXT),

            () => ValidateNumber(volunteer.ExperienceYears, "Experience years", minValue: 0, maxValue: 100),

            () => ValidateItems(volunteer.SocialNetworksDtos, SocialNetwork.Validate),

            () => ValidateItems(volunteer.DonateDetailsDtos, DonateDetails.Validate));
    }
}
