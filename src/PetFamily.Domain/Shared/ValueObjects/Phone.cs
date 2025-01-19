using PetFamily.Domain.Shared.DomainResult;
using static PetFamily.Domain.Shared.Validations.ValidationExtensions;
using static PetFamily.Domain.Shared.Validations.ValidationPatterns;
using static PetFamily.Domain.Shared.Validations.ValidationConstants;

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
            return Result<Phone>.Failure(validationResult.Errors!);

        return Result<Phone>.Success(new Phone(number!, regionCode!));
    }

    public static Result Validate(string? number, string? regionCode) =>

        Result.ValidateCollection(

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