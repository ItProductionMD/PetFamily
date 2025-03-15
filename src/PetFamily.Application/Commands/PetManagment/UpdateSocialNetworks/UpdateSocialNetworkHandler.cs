using Microsoft.Extensions.Logging;
using PetFamily.Application.IRepositories;
using PetFamily.Domain.Results;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.ValueObjects;
using static PetFamily.Domain.Shared.Validations.ValidationExtensions;

namespace PetFamily.Application.Commands.PetManagment.UpdateSocialNetworks;

public class UpdateSocialNetworkHandler(
    IVolunteerRepository volunteerRepository,
    ILogger<UpdateSocialNetworkHandler> logger)
{
    private readonly IVolunteerRepository _volunteerRepository = volunteerRepository;
    private readonly ILogger<UpdateSocialNetworkHandler> _logger = logger;
    public async Task<UnitResult> Handle(
        UpdateSocialNetworksCommand command,
        CancellationToken cancelToken)
    {
        var validationResult = ValidateItems(
            command.SocialNetworksDtos,
            (s)=>SocialNetworkInfo.Validate(s.Name,s.Url));

        if (validationResult.IsFailure)
        {
            _logger.LogError(
                "UpdateSocialNetworkHandler socials validation failure!{validationResult.Errors}",
                validationResult.ToErrorMessages());

            return validationResult;
        }
        //------------------------------------Get Volunteer---------------------------------------//
        var volunteer = await _volunteerRepository.GetByIdAsync(command.volunteerId, cancelToken);
        
        var socialNetworks = command.SocialNetworksDtos
            .Select(social => SocialNetworkInfo.Create(social.Name, social.Url).Data) ?? [];

        volunteer.UpdateSocialNetworks(socialNetworks!);

        await _volunteerRepository.Save(volunteer, cancelToken);

        _logger.LogInformation("Udate SocialNetworks for volunteer with id:{Id} successful.",
            command.volunteerId);

        return UnitResult.Ok();
    }
}
