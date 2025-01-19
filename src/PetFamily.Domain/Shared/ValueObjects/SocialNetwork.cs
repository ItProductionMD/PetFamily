using PetFamily.Domain.Shared.DomainResult;
using static PetFamily.Domain.Shared.Validations.ValidationExtensions;
using static PetFamily.Domain.Shared.Validations.ValidationConstants;
using static PetFamily.Domain.Shared.Validations.ValidationPatterns;
using PetFamily.Domain.Shared.DTO;

namespace PetFamily.Domain.Shared.ValueObjects;


public record SocialNetwork
{
    public string Name { get; }
    public string Url { get; }

    private SocialNetwork(string name, string url)
    {
        Name = name;
        Url = url;
    }

    public static Result<SocialNetwork?> Create(string? name, string? url)
    {
        if (IsSocialNetworkEmpty(name, url))
            return Result<SocialNetwork?>.Success(null);

        var validationResult = Validate(name, url);

        if (validationResult.IsFailure)
            return Result<SocialNetwork?>.Failure(validationResult.Errors!);

        return Result<SocialNetwork?>.Success(new SocialNetwork(name!, url!));
    }

    public static Result Validate(string? name, string? url)
    {
        if (IsSocialNetworkEmpty(name, url))
            return Result.Success();

        return Result.ValidateCollection(

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
    public static Result Validate(SocialNetworkDTO dto) => Validate(dto.Name, dto.Url);

    private static bool IsSocialNetworkEmpty(string? name, string? url)
    {
        return HasOnlyEmptyStrings(name, url);
    }
}