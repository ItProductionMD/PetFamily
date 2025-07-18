﻿using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using static PetFamily.SharedKernel.Validations.ValidationConstants;
using static PetFamily.SharedKernel.Validations.ValidationExtensions;
using static PetFamily.SharedKernel.Validations.ValidationPatterns;

namespace PetFamily.SharedKernel.ValueObjects;
public record Phone : IValueObject
{
    public string Number { get; }
    public string RegionCode { get; }

    private Phone() { }//EfCore need this
    private Phone(string number, string regionCode)
    {
        Number = number;
        RegionCode = regionCode;
    }

    public static Phone CreateEmpty() => new Phone("", "");

    public static Result<Phone> CreateNotEmpty(string? number, string? regionCode)
    {
        var validationResult = Validate(number, regionCode);
        if (validationResult.IsFailure)
            return validationResult;

        return Result.Ok(new Phone(number!, regionCode!));
    }

    public static Result<Phone> CreatePossibbleEmpty(string? number, string? regionCode)
    {
        var validationResult = ValidateNonRequired(number, regionCode);
        if (validationResult.IsFailure)
            return validationResult;

        if (IsPhoneEmpty(regionCode, number))
            return Result.Ok(CreateEmpty());

        return Result.Ok(new Phone(number!, regionCode!));
    }

    public static UnitResult Validate(string? number, string? regionCode) =>

        UnitResult.FromValidationResults(

            () => ValidateRequiredField(

                valueToValidate: number,
                valueName: "Phone number",
                maxLength: MAX_LENGTH_SHORT_TEXT,
                pattern: PHONE_NUMBER_PATTERN),

            () => ValidateRequiredField(

                valueToValidate: regionCode,
                valueName: "Phone regionCode",
                maxLength: MAX_LENGTH_SHORT_TEXT,
                pattern: PHONE_REGION_PATTERN));

    public static UnitResult ValidateNonRequired(string? number, string? regionCode)
    {
        if (IsPhoneEmpty(number, regionCode))
            return UnitResult.Ok();

        return Validate(number, regionCode);
    }

    private static bool IsPhoneEmpty(string? regionCode, string? number)
    {
        return HasOnlyEmptyStrings(regionCode, number);
    }

    public IEnumerable<object> GetEqualityComponents()
    {
        yield return Number;
        yield return RegionCode;
    }
    public override string ToString()
    {
        return $"{RegionCode}-{Number}";
    }

    public string ToStringFormattedPhone()
    {
        return $"{RegionCode}-{Number}";
    }

    public static Result<Phone> FromStringFormattedPhone(string formattedPhone)
    {
        var parts = formattedPhone.Split('-');
        if (parts.Length != 2)
            Result.Fail(Error.InvalidFormat("Phone"));

        return Result.Ok(new Phone(parts[1], parts[0]));
    }
}