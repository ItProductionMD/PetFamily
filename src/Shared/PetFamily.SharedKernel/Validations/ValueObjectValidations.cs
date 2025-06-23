using PetFamily.SharedKernel.Results;
using static PetFamily.SharedKernel.Validations.ValidationExtensions;
using static PetFamily.SharedKernel.Validations.ValidationConstants;
using static PetFamily.SharedKernel.Validations.ValidationPatterns;
using PetFamily.SharedKernel.ValueObjects;

namespace PetFamily.SharedKernel.Validations;

public static class ValueObjectValidations
{
    public static UnitResult ValidatePhone(string? regionCode, string? number) =>

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
}
