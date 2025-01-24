﻿using FluentValidation;
using Microsoft.AspNetCore.Identity;
using PetFamily.Application.Volunteers.CreateVolunteer;
using PetFamily.Domain.Shared.ValueObjects;
using static PetFamily.Domain.Shared.Validations.ValidationConstants;
using static PetFamily.Domain.Shared.Validations.ValidationPatterns;


namespace PetFamily.Application.Validations;

public class VolunteerRequestFluentValidator : AbstractValidator<CreateVolunteerRequest>
{
    public VolunteerRequestFluentValidator()
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
            .LessThanOrEqualTo(100);

        RuleForEach(c => c.SocialNetworksDtos)
            .MustBeValueObject(s => SocialNetwork.Validate(s.Name, s.Url));

        RuleForEach(c => c.DonateDetailsDtos)
           .MustBeValueObject(d => DonateDetails.Validate(d.Name, d.Description));
    }
}
