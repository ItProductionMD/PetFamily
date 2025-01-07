using PetFamily.Domain.DomainResult;
using PetFamily.Domain.Shared.Validations;

namespace PetFamily.Domain.Shared.ValueObjects
{
    public record SocialNetwork
    {
        public string Name { get; }
        public string Url { get; }
        private SocialNetwork(string name, string url)
        {
            Name = name;
            Url = url;
        }
        public static Result<SocialNetwork> Create(string? name, string? url)
        {
            var validationResult = Validate(name, url);
            if (validationResult.IsFailure)
                return Result<SocialNetwork>.Failure(validationResult.Errors);
            return Result<SocialNetwork>.Success(new SocialNetwork(name!, url!));
        }
        private static Result Validate(string? name, string? url)
        {
            var dictionaryToValidate = new Dictionary<string, string?>()
            {
                {"Name",name},
                {"Url",url}
            };
            var nullOrEmptyValidationResult = ValidationExtensions.ValidateIfStringsNotEmpty(dictionaryToValidate);
            if (nullOrEmptyValidationResult.IsFailure)
                return nullOrEmptyValidationResult;
            //TODO: Add more validations
            return Result.Success();
        }
    }
}
