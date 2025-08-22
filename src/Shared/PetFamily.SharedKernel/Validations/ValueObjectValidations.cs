using PetFamily.SharedKernel.Results;
using static PetFamily.SharedKernel.Validations.ValidationConstants;
using static PetFamily.SharedKernel.Validations.ValidationExtensions;
using static PetFamily.SharedKernel.Validations.ValidationPatterns;

namespace PetFamily.SharedKernel.Validations;

public static class ValueObjectValidations
{
    public static UnitResult ValidateRequiredPhone(string? regionCode, string? number) =>

        UnitResult.FromValidationResults(

            () => ValidateRequiredField(
                regionCode,
                "Phone region code",
                MAX_LENGTH_SHORT_TEXT,
                PHONE_REGION_PATTERN),

            () => ValidateRequiredField(
                number,
                "Phone number",
                MAX_LENGTH_SHORT_TEXT,
                PHONE_NUMBER_PATTERN));

    public static UnitResult ValidateFullName(string? firstName, string? lastName)
    {
        return UnitResult.FromValidationResults(

            () => ValidateRequiredField(lastName, "LastName", MAX_LENGTH_SHORT_TEXT, NAME_PATTERN),

            () => ValidateRequiredField(firstName, "FirstName", MAX_LENGTH_SHORT_TEXT, NAME_PATTERN)
        );
    }
}
