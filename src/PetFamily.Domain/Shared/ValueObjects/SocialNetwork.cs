using PetFamily.Domain.Shared.DomainResult;
using static PetFamily.Domain.Shared.Validations.ValidationExtensions;
using static PetFamily.Domain.Shared.Validations.ValidationConstants;
using static PetFamily.Domain.Shared.Validations.ValidationPatterns;

namespace PetFamily.Domain.Shared.ValueObjects;
public record SocialNetwork
{
    public string Name { get; }
    public string Url { get; }
    private const string NAME = "Socialnetwork name";
    private const string URL = "Socialnetwork url";
    private SocialNetwork(string name, string url)
    {
        Name = name;
        Url = url;
    }
    public static Result<SocialNetwork?> Create(string? name, string? url,bool isRequired)
    {
        var hasOnlyEmptyStrings = HasOnlyEmptyStrings(name, url);
        if (hasOnlyEmptyStrings && !isRequired)
            return Result<SocialNetwork?>.Success(null);
        var validationResult = Validate(name, url);
        if (validationResult.IsFailure)
            return Result<SocialNetwork?>.Failure(validationResult.Error!);
        return Result<SocialNetwork?>.Success(new SocialNetwork(name!, url!));
    }
    private static Result Validate(string? name, string? url) =>
        ValidateRequiredField(name, NAME, MAX_LENGTH_SHORT_TEXT, NAME_PATTERN)
        .OnFailure(() =>ValidateRequiredField(url,URL,MAX_LENGTH_SHORT_TEXT)); 
}
