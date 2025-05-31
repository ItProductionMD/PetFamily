using PetFamily.SharedKernel.Results;
using System.Text.Json.Serialization;
using static PetFamily.SharedKernel.Validations.ValidationConstants;
using static PetFamily.SharedKernel.Validations.ValidationExtensions;
using static PetFamily.SharedKernel.Validations.ValidationPatterns;

namespace PetFamily.SharedKernel.ValueObjects;


public record SocialNetworkInfo
{
    public string Name { get; }
    public string Url { get; }
    [JsonConstructorAttribute]
    private SocialNetworkInfo(string name, string url)
    {
        Name = name;
        Url = url;
    }
    public static Result<SocialNetworkInfo> Create(string? name, string? url)
    {
        if (IsSocialNetworkEmpty(name, url))
            return UnitResult.Ok();

        var validationResult = Validate(name, url);
        if (validationResult.IsFailure)
            return validationResult;

        return Result.Ok(new SocialNetworkInfo(name!, url!));
    }
    public static UnitResult Validate(string? name, string? url)
    {
        if (IsSocialNetworkEmpty(name, url))
            return UnitResult.Ok();

        return UnitResult.ValidateCollection(

            () => ValidateRequiredField(

                valueToValidate: name,
                valueName: "SocialNetwork name",
                maxLength: MAX_LENGTH_SHORT_TEXT,
                pattern: NAME_PATTERN),

            () => ValidateRequiredField(

                valueToValidate: url,
                valueName: "SocialNetwork url",
                maxLength: MAX_LENGTH_SHORT_TEXT));
    }
    private static bool IsSocialNetworkEmpty(string? name, string? url)
    {
        return HasOnlyEmptyStrings(name, url);
    }
}