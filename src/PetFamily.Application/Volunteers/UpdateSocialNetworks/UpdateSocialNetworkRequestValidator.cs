using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.DomainResult;
using PetFamily.Domain.Shared.ValueObjects;
using static PetFamily.Application.Volunteers.SharedVolunteerRequests;


namespace PetFamily.Application.Volunteers.UpdateSocialNetworks;

public class UpdateSocialNetworkRequestValidator
{
    public static Result Validate(IEnumerable<SocialNetworksRequest> socials)
    {
        List<Error?> errors = [];

        foreach (var social in socials)
        {
            var validateSocialNetwork = SocialNetwork.Validate(social.Name, social.Url);

            if (validateSocialNetwork.IsFailure)
                errors.AddRange(validateSocialNetwork.Errors!);
        }
        if (errors.Count > 0)
            return Result.Failure(errors);

        return Result.Success();
    }
}
