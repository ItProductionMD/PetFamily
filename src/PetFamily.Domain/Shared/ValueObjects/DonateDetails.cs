using PetFamily.Domain.Shared.DomainResult;
using static PetFamily.Domain.Shared.Validations.ValidationExtensions;
using static PetFamily.Domain.Shared.Validations.ValidationConstants;

namespace PetFamily.Domain.Shared.ValueObjects;

public record DonateDetails
{
    public string Name { get; }
    public string Description { get; }
    private const string NAME = "DonateDetails Name";
    private const string DESCRIPTION= "DonateDetails Description";
    private DonateDetails(string name, string description)
    {
        Name = name;
        Description = description;
    }
    public static Result<DonateDetails?> Create(string? name, string? description,bool isRequired)
    {
        var hasOnlyEmptyStrings = HasOnlyEmptyStrings(name,description);
        if (hasOnlyEmptyStrings && !isRequired)
            return Result<DonateDetails?>.Success(null);
        var validationResult = Validate(name, description);
        if (validationResult.IsFailure)
            return Result<DonateDetails?>.Failure(validationResult.Error!);
        return Result<DonateDetails?>.Success(new DonateDetails(name!, description!));
    }
    private static Result Validate(string? name, string? description)=>
        ValidateRequiredField(name, NAME, MAX_LENGTH_SHORT_TEXT)
        .OnFailure(() => ValidateRequiredField(description, DESCRIPTION, MAX_LENGTH_LONG_TEXT));        
}
