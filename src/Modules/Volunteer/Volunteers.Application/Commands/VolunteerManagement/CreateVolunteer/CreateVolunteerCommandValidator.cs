using FluentValidation;
using PetFamily.Application.Validations;
using PetFamily.SharedKernel.Validations;
using PetFamily.SharedKernel.ValueObjects;
using static PetFamily.SharedKernel.Validations.ValidationConstants;


namespace Volunteers.Application.Commands.VolunteerManagement.CreateVolunteer;

public class CreateVolunteerCommandValidator : AbstractValidator<CreateVolunteerCommand>
{
    public CreateVolunteerCommandValidator()
    {
        RuleFor(c => c.Email)
            .NotEmpty()
            .MaximumLength(MAX_LENGTH_SHORT_TEXT)
            .EmailAddress()
            .WithErrorCode("value.is.invalid");

        RuleFor(c => new { c.PhoneNumber, c.PhoneRegionCode })
            .MustBeValueObject(phone => Phone.Validate(phone.PhoneNumber, phone.PhoneRegionCode));

        RuleFor(c => new { c.FirstName, c.LastName })
            .MustBeValueObject(fullName => FullName.Validate(fullName.FirstName, fullName.LastName));

        RuleFor(c => c.Description).MaximumLength(MAX_LENGTH_LONG_TEXT)
            .WithMessage($"Description length is bigger than {MAX_LENGTH_LONG_TEXT}")
            .WithErrorCode("value.format.is.invalid");

        RuleFor(c => c.ExperienceYears)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(100)
            .WithMessage($"Value is bigger than 100 or less than 0")
            .WithErrorCode(ValidationErrorCodes.VALUE_OUT_OF_RANGE);

        RuleForEach(c => c.SocialNetworksList)
            .MustBeValueObject(s => SocialNetworkInfo.Validate(s.Name, s.Url));

        RuleForEach(c => c.Requisites)
           .MustBeValueObject(d => RequisitesInfo.Validate(d.Name, d.Description));
    }
}
