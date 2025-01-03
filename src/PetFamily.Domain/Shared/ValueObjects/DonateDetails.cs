using PetFamily.Domain.DomainResult;
using PetFamily.Domain.Shared.Validations;

namespace PetFamily.Domain.Shared.ValueObjects
{
    public record DonateDetails
    {
        public string Name { get; }
        public string Description { get; }

        private DonateDetails(string name, string description)
        {
            Name = name;
            Description = description;
        }
        public static Result<DonateDetails> Create(string? name, string? description)
        {
            var validationResult = Validate(name, description);
            if (validationResult.IsFailure)
                return Result<DonateDetails>.Failure(validationResult.Errors);
            return Result<DonateDetails>.Success(new DonateDetails(name!, description!));
        }
        private static Result Validate(string? name, string? description)
        {
            var dictionaryToValidate = new Dictionary<string, string?>()
            {
                { "Name", name },
                { "Description", description }
            };
            var nullOrEmptyValidationResult = ValidationExtensions.ValidateIfStringsNotEmpty(dictionaryToValidate);
            if (nullOrEmptyValidationResult.IsFailure)
                return nullOrEmptyValidationResult;
            //Todo: Add more validations
            return Result.Success();
        }
    }
}
