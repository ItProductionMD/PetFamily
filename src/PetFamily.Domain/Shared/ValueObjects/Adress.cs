using static PetFamily.Domain.Shared.Validations.ValidationExtensions;
using static PetFamily.Domain.Shared.Validations.ValidationConstants;
using static PetFamily.Domain.Shared.Validations.ValidationPatterns;
using PetFamily.Domain.Shared.DomainResult;

namespace PetFamily.Domain.Shared.ValueObjects;

// The check for an empty address is duplicated in both the Create and Validate methods.
// This duplication ensures that each method can operate independently.
// In the Create method, it prevents unnecessary validation when all fields are empty.
// In the Validate method, it allows reuse of validation logic in other contexts without relying on Create.
// While this duplication may seem redundant, it ensures consistency and flexibility for different use cases.
public record Adress
{
    public string Street { get; }
    public string City { get; }
    public string Region { get; }
    public string Number { get; }

    private Adress() { }//EF core need this

    private Adress(string region, string city, string street, string number)
    {
        Region = region;
        City = city;
        Street = street;
        Number = number;
    }

    public static Result<Adress?> Create(string? region, string? city, string? street, string? number)
    {       

        if (IsAdressEmpty(region, city, street, number))
            return Result<Adress?>.Success(null);

        var validationResult = Validate(region, city, street, number);

        if (validationResult.IsFailure)
            return Result<Adress?>.Failure(validationResult.Errors!);

        return Result<Adress?>.Success(new Adress(region!, city!, street!, number!));
    }

    public static Result Validate(string? region, string? city, string? street, string? number)
    {
        if (IsAdressEmpty(region, city, street, number))
            return Result.Success();

        return Result.ValidateCollection(

            () => ValidateRequiredField(
                valueToValidate: region,
                valueName: "Adress region",
                maxLength: MAX_LENGTH_SHORT_TEXT,
                pattern: NAME_PATTERN),

            () => ValidateRequiredField(
                valueToValidate: city,
                valueName: "Adress city",
                maxLength: MAX_LENGTH_SHORT_TEXT,
                pattern: NAME_PATTERN),

            () => ValidateRequiredField(
                valueToValidate: street,
                valueName: "Adress street",
                maxLength: MAX_LENGTH_SHORT_TEXT,
                pattern: STREET_PATTERN),

            () => ValidateRequiredField(
                valueToValidate: number,
                valueName: "Adress number",
                maxLength: MAX_LENGTH_SHORT_TEXT,
                pattern: ADRESS_NUMBER_PATTERN));
    }

    private static bool IsAdressEmpty(string? region, string? city, string? street, string? number)
    {
        return HasOnlyEmptyStrings(region, city, street, number);
    }
}