using PetFamily.SharedKernel.Results;
using System.Text.Json.Serialization;
using static PetFamily.SharedKernel.Validations.ValidationConstants;
using static PetFamily.SharedKernel.Validations.ValidationExtensions;

namespace PetFamily.SharedKernel.ValueObjects;

public record Image
{
    public string Name { get; }

    [JsonConstructorAttribute]
    private Image(string name)
    {
        Name = name;
    }
    public static Result<Image> Create(string name)
    {
        var validationResult = Validate(name);
        if (validationResult.IsFailure)
            return validationResult;

        return Result.Ok(new Image(name));
    }
    public static UnitResult Validate(string name)
    {
        var validationResult = ValidateRequiredField(name, "Image Name", MAX_LENGTH_SHORT_TEXT);
        return validationResult;
    }
}
