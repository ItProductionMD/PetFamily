using static PetFamily.Domain.Shared.Validations.ValidationExtensions;
using static PetFamily.Domain.Shared.Validations.ValidationConstants;
using PetFamily.Domain.Results;
using System.Text.Json.Serialization;

namespace PetFamily.Domain.Shared.ValueObjects;

public record Image
{
    public string Name { get;}

    [JsonConstructorAttribute]
    private Image(string name)
    {
        Name = name;
    }
    public static Result<Image> Create(string name)
    {
        var validationResult = Validate(name);
        if(validationResult.IsFailure)
            return validationResult;

        return Result.Ok(new Image (name));
    }
    public static UnitResult Validate(string name)
    {
        var validationResult = ValidateRequiredField(name, "Image Name",MAX_LENGTH_MEDIUM_TEXT);
        return validationResult;
    }
}
