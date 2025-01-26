using FluentValidation;
using PetFamily.Application.Validations;
using PetFamily.Domain.Shared.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PetFamily.Domain.Shared.Validations.ValidationConstants;
using static PetFamily.Domain.Shared.Validations.ValidationCodes;


namespace PetFamily.Application.Volunteers.UpdateVolunteer
{
    public class UpdateVolunteerRequestValidator : AbstractValidator<UpdateVolunteerRequest>
    {
        public UpdateVolunteerRequestValidator()
        {
            RuleFor(c => c.Email)
               .NotEmpty()
               .MaximumLength(MAX_LENGTH_SHORT_TEXT)
               .EmailAddress()
               .WithErrorCode(INVALID_VALUE_CODE);

            RuleFor(c => new { c.PhoneNumber, c.PhoneRegionCode })
                .MustBeValueObject(phone =>
                    Phone.Validate(phone.PhoneNumber, phone.PhoneRegionCode));

            RuleFor(c => new { c.FirstName, c.LastName })
                .MustBeValueObject(fullName =>
                    FullName.Validate(fullName.FirstName, fullName.LastName));

            RuleFor(c => c.Description).MaximumLength(MAX_LENGTH_LONG_TEXT)
                .WithMessage($"Description length is bigger than {MAX_LENGTH_LONG_TEXT}")
                .WithErrorCode(INVALID_VALUE_CODE);


            RuleFor(c => c.ExperienceYears)
                .GreaterThanOrEqualTo(0)
                .LessThanOrEqualTo(100)
                .WithMessage($"Value is bigger than 100 or less than 0")
                .WithErrorCode(INVALID_VALUE_CODE);

        }
    }
}
