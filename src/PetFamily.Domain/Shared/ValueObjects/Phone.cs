using PetFamily.Domain.DomainResult;
using PetFamily.Domain.Shared.Validations;

namespace PetFamily.Domain.Shared.ValueObjects
{
    public record Phone
    {
        public string Number { get; }
        public string RegionCode { get; }
        private Phone(string number, string regionCode)
        {
            Number = number;
            RegionCode = regionCode;
        }
        public static Result<Phone> Create(string? number, string? regionCode)
        {
            var validationResult = Validate(number, regionCode);
            if (validationResult.IsFailure)
                return Result<Phone>.Failure(validationResult.Errors);
            return Result<Phone>.Success(new Phone(number!, regionCode!));
        }
        private static Result Validate(string? number, string? regionCode)
        {
            var dictionaryToValidate = new Dictionary<string, string?>()
            {
                { "Number", number },
                { "RegionCode", regionCode }
            };
            var nullOrEmptyValidationResult = ValidationExtensions.ValidateIfStringsNotEmpty(dictionaryToValidate);
            if (nullOrEmptyValidationResult.IsFailure)
                return nullOrEmptyValidationResult;
            //Todo: Add more validations
            return Result.Success();
        }
    }
}