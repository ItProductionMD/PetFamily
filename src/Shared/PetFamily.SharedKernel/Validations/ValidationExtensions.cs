using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using System.Text.RegularExpressions;
using static PetFamily.SharedKernel.Errors.Error;

namespace PetFamily.SharedKernel.Validations;
public static class ValidationExtensions
{
    public static UnitResult ValidateRequiredField(
        string? valueToValidate,
        string valueName,
        int maxLength,
        string? pattern = null)
    {
        if (string.IsNullOrWhiteSpace(valueToValidate))
            return UnitResult.Fail(StringIsNullOrEmpty(valueName));

        valueToValidate = valueToValidate.Trim();

        if (valueToValidate.Length > maxLength)
            return UnitResult.Fail(InvalidLength(valueName));

        if (pattern != null && !Regex.IsMatch(valueToValidate, pattern))
            return UnitResult.Fail(InvalidFormat(valueName));

        return UnitResult.Ok();
    }

    public static UnitResult ValidateNonRequiredField(
        string? valueToValidate,
        string valueName,
        int maxLength,
        string? pattern = null)
    {
        return string.IsNullOrWhiteSpace(valueToValidate)
            ? UnitResult.Ok()
            : ValidateRequiredField(valueToValidate, valueName, maxLength, pattern);
    }

    public static UnitResult ValidateRequiredObject<T>(T? objToValidate, string valueName) =>
        objToValidate != null
            ? UnitResult.Ok()
            : UnitResult.Fail(Error.StringIsNullOrEmpty(valueName));

    public static bool HasOnlyEmptyStrings(params string?[] strings) =>
         !strings.Any(s => !string.IsNullOrWhiteSpace(s));

    public static UnitResult ValidateIntegerNumber(int number, string valueName, int minValue, int maxValue)
    {
        if (number < minValue || number > maxValue)
            return UnitResult.Fail(InvalidFormat(valueName));

        return UnitResult.Ok();
    }
    public static UnitResult ValidateNumber<T>(T number, string valueName, T minValue, T maxValue)
        where T : IComparable<T>
    {
        if (number.CompareTo(minValue) < 0 || number.CompareTo(maxValue) > 0)
            return UnitResult.Fail(ValueOutOfRange(valueName));

        return UnitResult.Ok();
    }
    public static UnitResult ValidateIfGuidIsNotEpmty(Guid id, string valueName)
    {
        return id == Guid.Empty
            ? UnitResult.Fail(Error.InvalidFormat(valueName))
            : UnitResult.Ok();
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
        var validationErrors = new List<ValidationError>();
        foreach (var item in items)
        {
            var unitResult = itemValidator(item);
            if (unitResult.IsFailure)
                validationErrors.AddRange(unitResult.Error.ValidationErrors);
        }
        return validationErrors.Count > 0
            ? UnitResult.Fail(Error.FromValidationErrors(validationErrors))
            : UnitResult.Ok();
    }
}
