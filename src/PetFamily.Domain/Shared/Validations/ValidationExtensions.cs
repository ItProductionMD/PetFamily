using System.Text.RegularExpressions;
using PetFamily.Domain.Shared.DomainResult;
using static PetFamily.Domain.Shared.Error;

namespace PetFamily.Domain.Shared.Validations;

public static class ValidationExtensions
{
    public static Result ValidateRequiredField(string? value,string valueName,int maxLength, string pattern)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure(CreateErrorStringNullOrEmpty(valueName));

        if (value.Length > maxLength)
            return Result.Failure(CreateErrorInvalidLength(valueName));

        if (!Regex.IsMatch(value, pattern))
            return Result.Failure(CreateErrorInvalidFormat(valueName));

        return Result.Success();
    }

    public static Result ValidateRequiredField(string? value, string valueName, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure(CreateErrorStringNullOrEmpty(valueName));

        if (value.Length > maxLength)
            return Result.Failure(CreateErrorInvalidLength(valueName));

        return Result.Success();
    }

    public static Result ValidateNonRequiredField(string? value, string valueName, int maxLength)
    {
        if (!string.IsNullOrWhiteSpace(value))
            ValidateRequiredField(value, valueName, maxLength);

        return Result.Success();
    }

    public static Result ValidateRequiredObject<T>(this T? obj, string valueName) =>

        obj != null ? Result.Success() : Result.Failure(CreateErrorValueRequired(valueName));
   
    public static bool HasOnlyEmptyStrings(params string?[] strings) =>

         !strings.Any(s => !string.IsNullOrWhiteSpace(s));
}
