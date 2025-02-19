using static PetFamily.Domain.Shared.Validations.ValidationExtensions;
using static PetFamily.Domain.Shared.Validations.ValidationPatterns;
using static PetFamily.Domain.Shared.Validations.ValidationConstants;
using PetFamily.Domain.Results;

namespace PetFamily.Domain.Shared.ValueObjects;
public record Phone
{
    public string Number { get; }
    public string RegionCode { get; }

    private Phone(string number, string regionCode)
    {
        Number = number;
        RegionCode = regionCode;
    }

    public static Result<Phone> Create(string? number, string? regionCode)
    {
        var validationResult = Validate(number, regionCode);
        if (validationResult.IsFailure)
            return validationResult;

        return Result.Ok(new Phone(number!, regionCode!));
    }

    public static UnitResult Validate(string? number, string? regionCode) =>

        UnitResult.ValidateCollection(

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

}