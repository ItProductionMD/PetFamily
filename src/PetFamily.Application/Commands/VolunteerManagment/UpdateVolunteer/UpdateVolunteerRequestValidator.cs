using FluentValidation;
using PetFamily.Application.Validations;
using PetFamily.Domain.Shared.Validations;
using PetFamily.Domain.Shared.ValueObjects;
using static PetFamily.Domain.Shared.Validations.ValidationConstants;


namespace PetFamily.Application.Commands.VolunteerManagment.UpdateVolunteer;

public class UpdateVolunteerRequestValidator : AbstractValidator<UpdateVolunteerCommand>
{
    public UpdateVolunteerRequestValidator()
    {
        RuleFor(c => c.Email)
           .NotEmpty()
           .MaximumLength(MAX_LENGTH_SHORT_TEXT)
           .EmailAddress()
           .WithErrorCode(ValidationErrorCodes.VALUE_INVALID_LENGTH);

        RuleFor(c => new { c.PhoneNumber, c.PhoneRegionCode })
            .MustBeValueObject(phone => Phone.Validate(phone.PhoneNumber, phone.PhoneRegionCode));

        RuleFor(c => new { c.FirstName, c.LastName })
            .MustBeValueObject(fullName => FullName.Validate(fullName.FirstName, fullName.LastName));

        RuleFor(c => c.Description).MaximumLength(MAX_LENGTH_LONG_TEXT)
            .WithMessage($"Description length is bigger than {MAX_LENGTH_LONG_TEXT}")
            .WithErrorCode(ValidationErrorCodes.VALUE_INVALID_LENGTH);

        RuleFor(c => c.ExperienceYears)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(100)
            .WithMessage($"Value is bigger than 100 or less than 0")
            .WithErrorCode(ValidationErrorCodes.VALUE_OUT_OF_RANGE);

    }
}
