using PetFamily.Domain.DomainResult;

namespace PetFamily.Domain.Shared.Validations
{
    public static class ValidationExtensions
    {
        public static Result ValidateIfStringNotEmpty(string valueName, string? stringValue)
        {
            if (string.IsNullOrWhiteSpace(stringValue))
                return Result.Failure($"{valueName} cannot be null, empty, or whitespace.");
            return Result.Success();
        }
        public static Result ValidateIfStringsNotEmpty(Dictionary<string, string?> nameAndValue)
        {
            var mainResult = Result.Success();
            foreach (var nameValue in nameAndValue)
            {
                var validationResult = ValidateIfStringNotEmpty(nameValue.Key, nameValue.Value);
                if (validationResult.IsFailure)
                    mainResult.AddError(validationResult.Errors[0]);               
            }
            return mainResult;
        }
    }
}
