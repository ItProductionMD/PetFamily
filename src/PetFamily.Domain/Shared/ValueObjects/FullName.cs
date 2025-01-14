﻿using PetFamily.Domain.Shared.DomainResult;
using static PetFamily.Domain.Shared.Validations.ValidationExtensions;
using static PetFamily.Domain.Shared.Validations.ValidationConstants;
using static PetFamily.Domain.Shared.Validations.ValidationPatterns;

namespace PetFamily.Domain.Shared.ValueObjects
{

    public record FullName
    {
        public string FirstName { get; }
        public string LastName { get; }

        private FullName(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }

        public static Result<FullName> Create(string? firstName, string? lastName)
        {
            var validationResult = Validate(firstName, lastName);
            if (validationResult.IsFailure)
                return Result<FullName>.Failure(validationResult.Error!);

            return Result<FullName>.Success(new FullName(firstName!, lastName!));
        }

        private static Result Validate(string? firstName, string? lastName) =>

            ValidateRequiredField(lastName, "LastName", MAX_LENGTH_SHORT_TEXT, NAME_PATTERN)

            .OnFailure(() => 
                ValidateRequiredField(firstName, "FirstName", MAX_LENGTH_SHORT_TEXT, NAME_PATTERN));

    }
}