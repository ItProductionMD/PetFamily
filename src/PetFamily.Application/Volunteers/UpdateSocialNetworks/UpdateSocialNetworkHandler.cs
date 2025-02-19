using Microsoft.Extensions.Logging;
using PetFamily.Domain.Results;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.ValueObjects;
using static PetFamily.Application.Volunteers.SharedVolunteerRequests;
using static PetFamily.Domain.Shared.Validations.ValidationExtensions;

namespace PetFamily.Application.Volunteers.UpdateSocialNetworks;

public class UpdateSocialNetworkHandler(
    IVolunteerRepository volunteerRepository,
    ILogger<UpdateSocialNetworkHandler> logger)
{
    private readonly IVolunteerRepository _volunteerRepository = volunteerRepository;
    private readonly ILogger<UpdateSocialNetworkHandler> _logger = logger;
    public async Task<UnitResult> Handle(
        Guid volunteerId,
        IEnumerable<SocialNetworksRequest> socials,
        CancellationToken cancellationToken)
    {
        //--------------------------------------Validator-----------------------------------------//
        var validationResult = ValidateItems(socials,(s)=>SocialNetworkInfo.Validate(s.Name,s.Url));
        if (validationResult.IsFailure)
        {
            _logger.LogError(
                "UpdateSocialNetworkHandler socials validation failure!{validationResult.Errors}",
                validationResult.ConcateErrorMessages());

            return validationResult;
        }
        //------------------------------------Get Volunteer---------------------------------------//
        var getVolunteer = await _volunteerRepository.GetById(volunteerId, cancellationToken);
        if (getVolunteer.IsFailure)
        {
            _logger.LogError("Fail get volunteer with id {volunteerId} for update SocialNetworks!",
                volunteerId);

            return UnitResult.Fail(getVolunteer.Errors!);
        }
        var volunteer = getVolunteer.Data!;

        var socialNetworks = socials
            .Select(social => SocialNetworkInfo.Create(social.Name, social.Url).Data) ?? [];

        volunteer.UpdateSocialNetworks(socialNetworks!);

        await _volunteerRepository.Save(volunteer, cancellationToken);

        _logger.LogInformation("Udate SocialNetworks for volunteer with id:{Id} successful.",
            volunteerId);

        return UnitResult.Ok();
    }
}
