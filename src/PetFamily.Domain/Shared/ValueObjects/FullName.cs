using PetFamily.Domain.DomainResult;
using PetFamily.Domain.Shared.Validations;

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
                return Result<FullName>.Failure(validationResult.Errors);
            return Result<FullName>.Success(new FullName(firstName!, lastName!));
        }
        private static Result Validate(string? firstName, string? lastName)
        {
            var dictionaryToValidate = new Dictionary<string, string?>()
            {
                {"Firstname", firstName},
                {"Lastname", lastName}
            };
            var nullOrEmptyValidationResult = ValidationExtensions.ValidateIfStringsNotEmpty(dictionaryToValidate);
            if (nullOrEmptyValidationResult.IsFailure)
                return nullOrEmptyValidationResult;
            //TODO: Add more validations
            return Result.Success();
        }
    }
}
