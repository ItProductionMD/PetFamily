using Microsoft.Extensions.Logging;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.DomainResult;
using PetFamily.Domain.Shared.ValueObjects;
using static PetFamily.Application.Volunteers.SharedVolunteerRequests;

namespace PetFamily.Application.Volunteers.UpdateSocialNetworks;

public class UpdateSocialNetworkHandler(
    IVolunteerRepository volunteerRepository,
    ILogger<UpdateSocialNetworkHandler> logger)
{
    private readonly IVolunteerRepository _volunteerRepository = volunteerRepository;
    private readonly ILogger<UpdateSocialNetworkHandler> _logger = logger;
    public async Task<Result> Handle(
        Guid volunteerId,
        IEnumerable<SocialNetworksRequest> socials,
        CancellationToken cancellationToken)
    {
        //--------------------------------------Validator-----------------------------------------//
        var validationResult = UpdateSocialNetworkRequestValidator.Validate(socials);
        if (validationResult.IsFailure)
        {
            _logger.LogError(
                "UpdateSocialNetworkHandler socials validation failure!{validationResult.Errors}",
                validationResult.Errors);
            return Result.Failure(validationResult.Errors!);
        }
        //------------------------------------Get Volunteer---------------------------------------//
        var getVolunteer = await _volunteerRepository.GetById(volunteerId, cancellationToken);
        if (getVolunteer.IsFailure)
        {
            _logger.LogError("Volunteer with id {volunteerId} not found", volunteerId);
            return Result.Failure(getVolunteer.Errors!);
        }
        var volunteer = getVolunteer.Data;
        //-------------------------Create ValueObjectList<SocialNetwork>--------------------------//
        var socialNetworks = socials
            .Select(social => SocialNetwork.Create(social.Name, social.Url).Data) ?? [];
        //--------------------------------------Update Volunteer----------------------------------//
        volunteer.UpdateSocialNetworks(socialNetworks!);
        //--------------------------------------Save Volunteer------------------------------------//
        await _volunteerRepository.Save(volunteer, cancellationToken);

        return Result.Success();
    }
}
