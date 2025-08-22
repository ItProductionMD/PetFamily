using PetFamily.SharedApplication.Exceptions;
using PetFamily.SharedKernel.ValueObjects;
using Volunteers.Application.Commands.PetManagement.UpdateSocialNetworks;
using static PetFamily.SharedKernel.Validations.ValidationExtensions;


namespace Volunteers.Application.Commands.PetManagement.UpdateSocials;

public static class UpdateSocialNetworksValidator
{
    public static void Validate(this UpdateSocialNetworksCommand cmd)
    {
        var result = ValidateItems(
             cmd.SocialNetworksDtos,
             (s) => SocialNetworkInfo.Validate(s.Name, s.Url));

        if (result.IsFailure)
            throw new ValidationException(result.Error);
    }
}
