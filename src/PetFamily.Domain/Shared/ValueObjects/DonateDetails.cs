using PetFamily.Domain.Shared.DomainResult;
using static PetFamily.Domain.Shared.Validations.ValidationExtensions;
using static PetFamily.Domain.Shared.Validations.ValidationConstants;
using PetFamily.Domain.Shared.DTO;

namespace PetFamily.Domain.Shared.ValueObjects;

public record DonateDetails
{
    public string Name { get; }
    public string Description { get; }

    private DonateDetails(string name, string description)
    {
        Name = name;
        Description = description;
    }

    public static Result<DonateDetails?> Create(string? name, string? description)
    {
        if (HasOnlyEmptyStrings(name, description))
            return Result<DonateDetails?>.Success(null);

        var validationResult = Validate(name, description);

        if (validationResult.IsFailure)
            return Result<DonateDetails?>.Failure(validationResult.Errors!);

        return Result<DonateDetails?>.Success(new DonateDetails(name!, description!));
    }

    public static Result Validate(string? name, string? description) =>

        Result.ValidateCollection(

            () => ValidateRequiredField(

                valueToValidate: name,
                valueName: "DonateDetails name",
                maxLength: MAX_LENGTH_SHORT_TEXT,
                pattern: null),

            () => ValidateRequiredField(

                valueToValidate: description,
                valueName: "DonateDetails description",
                maxLength: MAX_LENGTH_LONG_TEXT,
                pattern: null));

    public static Result Validate(DonateDetailsDTO dto) => Validate(dto.Name, dto.Description);


}