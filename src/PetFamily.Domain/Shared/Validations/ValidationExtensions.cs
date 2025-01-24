using System.Text.RegularExpressions;
using PetFamily.Domain.Shared.DomainResult;
using static PetFamily.Domain.Shared.Error;

namespace PetFamily.Domain.Shared.Validations;

public static class ValidationExtensions
{
    public delegate Result ListValidatorDelegate<T1, TResult>(T1 arg1);

    public static Result ValidateRequiredField(
        string? valueToValidate,
        string valueName,
        int maxLength, 
        string? pattern =null)
    {
        if (string.IsNullOrWhiteSpace(valueToValidate))
            return Result.Failure(CreateErrorStringNullOrEmpty(valueName));

        valueToValidate = valueToValidate.Trim();

        if (valueToValidate.Length > maxLength)
            return Result.Failure(CreateErrorInvalidLength(valueName));

        if (pattern!=null && !Regex.IsMatch(valueToValidate, pattern))
            return Result.Failure(CreateErrorInvalidFormat(valueName));

        return Result.Success();
    }

    public static Result ValidateNonRequiredField(
        string? valueToValidate,
        string valueName,
        int maxLength, 
        string? pattern = null)
    {
        if (!string.IsNullOrWhiteSpace(valueToValidate))
            ValidateRequiredField(valueToValidate, valueName, maxLength,pattern);

        return Result.Success();
    }

    public static Result ValidateRequiredObject<T>(this T? objToValidate, string valueName) =>

        objToValidate != null ? Result.Success() : Result.Failure(CreateErrorValueRequired(valueName));
   
    public static bool HasOnlyEmptyStrings(params string?[] strings) =>

         !strings.Any(s => !string.IsNullOrWhiteSpace(s));

    public static Result ValidateNumber(int number, string valueName ,int minValue,int maxValue) 
    {
        if (number < minValue || number > maxValue)    
            return Result.Failure(CreateErrorInvalidFormat(valueName));
        
        return Result.Success();
    }

    /// <summary>
    /// Validates a collection of objects using a provided validation delegate for each item.
    /// </summary>
    /// <typeparam name="T">The type of the objects in the collection.</typeparam>
    /// <param name="items">The collection of items to validate.</param>
    /// <param name="itemValidator">A delegate that validates a single item and returns a <see cref="Result"/>.</param>
    /// <returns>
    /// A <see cref="Result"/> indicating success if all items are valid, or failure with a list of errors if any validation fails.
    /// </returns>
    public static Result ValidateItems<T>(IEnumerable<T> items, Func<T, Result> itemValidator)
    {
        var errors = new List<Domain.Shared.Error>();

        foreach (var item in items)
        {
            var validationResult = itemValidator(item);

            if (validationResult.IsFailure)
                errors.AddRange(validationResult.Errors);
        }
        return errors.Count > 0 ? Result.Failure(errors) : Result.Success();
    }

}
