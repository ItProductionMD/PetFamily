using System.Numerics;
using System.Text.RegularExpressions;
using PetFamily.Domain.Results;
using static PetFamily.Domain.DomainError.Error;
using PetFamily.Domain.DomainError;

namespace PetFamily.Domain.Shared.Validations;

public static class ValidationExtensions
{
    public static UnitResult ValidateRequiredField(
        string? valueToValidate,
        string valueName,
        int maxLength, 
        string? pattern =null)
    {
        if (string.IsNullOrWhiteSpace(valueToValidate))
            return UnitResult.Fail(StringIsNullOrEmpty(valueName));

        valueToValidate = valueToValidate.Trim();

        if (valueToValidate.Length > maxLength)
            return UnitResult.Fail(InvalidLength(valueName));

        if (pattern!=null && !Regex.IsMatch(valueToValidate, pattern))
            return UnitResult.Fail(InvalidFormat(valueName));

        return UnitResult.Ok();
    }

    public static UnitResult ValidateNonRequiredField(
        string? valueToValidate,
        string valueName,
        int maxLength, 
        string? pattern = null)
    {
        if (!string.IsNullOrWhiteSpace(valueToValidate))
            ValidateRequiredField(valueToValidate, valueName, maxLength,pattern);

        return UnitResult.Ok();
    }

    public static UnitResult ValidateRequiredObject<T>(T? objToValidate, string valueName) =>

        objToValidate != null ? UnitResult.Ok() : UnitResult.Fail(ValueIsRequired(valueName));
   
    public static bool HasOnlyEmptyStrings(params string?[] strings) =>

         !strings.Any(s => !string.IsNullOrWhiteSpace(s));

    public static UnitResult ValidateIntegerNumber(int number, string valueName ,int minValue,int maxValue) 
    {
        if (number < minValue || number > maxValue)    
            return UnitResult.Fail(InvalidFormat(valueName));
        
        return UnitResult.Ok();
    }
    public static UnitResult ValidateNumber<T>(T number, string valueName, T minValue, T maxValue)
        where T : IComparable<T>
    {
        if (number.CompareTo(minValue)<0 || number.CompareTo(maxValue) > 0)
            return UnitResult.Fail(InvalidFormat(valueName));

        return UnitResult.Ok();
    }
    /// <summary>
    /// Validates a collection of objects using a provided validation delegate for each item.
    /// </summary>
    /// <typeparam name="T">The type of the objects in the collection.</typeparam>
    /// <param name="items">The collection of items to validate.</param>
    /// <param name="itemValidator">A delegate that validates a single item and returns a <see cref="UnitResult"/>.</param>
    /// <returns>
    /// A <see cref="UnitResult"/> indicating success if all items are valid, or failure with a list of errors if any validation fails.
    /// </returns>
    public static UnitResult ValidateItems<T>(IEnumerable<T> items, Func<T, UnitResult> itemValidator)
    {
        var errors = new List<Error>();
        foreach (var item in items)
        {
            var validationResult = itemValidator(item);
            if (validationResult.IsFailure)
                errors.AddRange(validationResult.Errors);
        }
        return errors.Count > 0 ? UnitResult.Fail(errors) : UnitResult.Ok();
    }

}
