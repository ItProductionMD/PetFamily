﻿using static PetFamily.Domain.Shared.Validations.ValidationExtensions;
using static PetFamily.Domain.Shared.Validations.ValidationConstants;
using static PetFamily.Domain.Shared.Validations.ValidationPatterns;
using PetFamily.Domain.Results;

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
    public static Address CreateEmpty() => new Address("","","","");
    public static Result<Address> CreatePossibleEmpty(string? region, string? city, string? street, string? number)
    {
        if (IsAdressEmpty(region, city, street, number))
            return Result.Ok(CreateEmpty());
 
        var validationResult = ValidateNonRequired(region, city, street, number);

        if (validationResult.IsFailure)
            return validationResult;

        return Result.Ok(new Address(region!, city!, street!, number!));
    }

    public static UnitResult ValidateNonRequired(string? region, string? city, string? street, string? number)
    {
        if (IsAdressEmpty(region, city, street, number))
            return UnitResult.Ok();

        return UnitResult.ValidateCollection(

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