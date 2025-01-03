using PetFamily.Domain.DomainResult;
using PetFamily.Domain.Shared.Validations;

namespace PetFamily.Domain.Shared.ValueObjects
{
    public record PhoneNumber
    {
        public string Number { get; }
        public string RegionCode { get; }
        private PhoneNumber(string number, string regionCode)
        {
            Number = number;
            RegionCode = regionCode;
        }
        public static Result<PhoneNumber> Create(string? number, string? regionCode)
        {
            var validationResult = Validate(number, regionCode);
            if (validationResult.IsFailure)
                return Result<PhoneNumber>.Failure(validationResult.Errors);
            return Result<PhoneNumber>.Success(new PhoneNumber(number!, regionCode!));
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