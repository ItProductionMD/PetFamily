using PetFamily.Domain.Results;
using static PetFamily.Domain.Shared.Validations.ValidationConstants;
using static PetFamily.Domain.Shared.Validations.ValidationExtensions;
using static PetFamily.Domain.Shared.Validations.ValidationPatterns;

namespace PetFamily.Domain.Shared.ValueObjects;

// The check for an empty address is duplicated in both the Create and Validate methods.
// This duplication ensures that each method can operate independently.
// In the Create method, it prevents unnecessary validation when all fields are empty.
// In the Validate method, it allows reuse of validation logic in other contexts without relying on Create.
// While this duplication may seem redundant, it ensures consistency and flexibility for different use cases.
public record Address
{
    public string Street { get; }
    public string City { get; }
    public string Region { get; }
    public string Number { get; }

    private Address() { }//EF core need this

    private Address(string region, string city, string street, string number)
    {
        Region = region;
        City = city;
        Street = street;
        Number = number;
    }
    public static Address CreateEmpty() => new Address("", "", "", "");
    public static Result<Address> CreatePossibleEmpty(string? region, string? city, string? street, string? number)
    {
        if (IsAddressEmpty(region, city, street, number))
            return Result.Ok(CreateEmpty());

        var validationResult = ValidateRequired(region, city, street, number);

        if (validationResult.IsFailure)
            return validationResult;

        return Result.Ok(new Address(region!, city!, street!, number!));
    }

    public static UnitResult ValidateRequired(string? region, string? city, string? street, string? number)
    {
        return UnitResult.ValidateCollection(

            () => ValidateRequiredField(
                valueToValidate: region,
                valueName: "Address region",
                maxLength: MAX_LENGTH_SHORT_TEXT,
                pattern: STREET_PATTERN),

            () => ValidateRequiredField(
                valueToValidate: city,
                valueName: "Address city",
                maxLength: MAX_LENGTH_SHORT_TEXT,
                pattern: STREET_PATTERN),

            () => ValidateRequiredField(
                valueToValidate: street,
                valueName: "Address street",
                maxLength: MAX_LENGTH_SHORT_TEXT,
                pattern: STREET_PATTERN),

            () => ValidateRequiredField(
                valueToValidate: number,
                valueName: "Address number",
                maxLength: MAX_LENGTH_SHORT_TEXT,
                pattern: ADRESS_NUMBER_PATTERN));
    }

    public static UnitResult ValidateNonRequired(string? region, string? city, string? street, string? number)
    {
        if (IsAddressEmpty(region, city, street, number))
            return UnitResult.Ok();

        return ValidateRequired(region, city, street, number);
    }

    private static bool IsAddressEmpty(string? region, string? city, string? street, string? number)
    {
        return HasOnlyEmptyStrings(region, city, street, number);
    }
}