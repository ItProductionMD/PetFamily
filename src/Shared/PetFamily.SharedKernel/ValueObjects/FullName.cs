using PetFamily.SharedKernel.Results;
using static PetFamily.SharedKernel.Validations.ValidationConstants;
using static PetFamily.SharedKernel.Validations.ValidationExtensions;
using static PetFamily.SharedKernel.Validations.ValidationPatterns;

namespace PetFamily.SharedKernel.ValueObjects;
public record FullName
{
    public string FirstName { get; }
    public string LastName { get; }

    private FullName(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    public static Result<FullName> Create(string? firstName, string? lastName)
    {
        var validationResult = Validate(firstName, lastName);
        if (validationResult.IsFailure)
            return validationResult;

        return Result.Ok(new FullName(firstName!, lastName!));
    }

    public static UnitResult Validate(string? firstName, string? lastName) =>

        UnitResult.ValidateCollection(

            () => ValidateRequiredField(lastName, "LastName", MAX_LENGTH_SHORT_TEXT, NAME_PATTERN),

            () => ValidateRequiredField(firstName, "FirstName", MAX_LENGTH_SHORT_TEXT, NAME_PATTERN));

    public override string ToString() => string.Join(' ', LastName, FirstName);
}