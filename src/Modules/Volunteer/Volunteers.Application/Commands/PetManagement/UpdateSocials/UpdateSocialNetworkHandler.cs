using Microsoft.Extensions.Logging;
using PetFamily.Application.Abstractions.CQRS;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.ValueObjects;
using Volunteers.Application.IRepositories;
using static PetFamily.SharedKernel.Validations.ValidationExtensions;

namespace Volunteers.Application.Commands.PetManagement.UpdateSocialNetworks;

public class UpdateSocialNetworkHandler(
    IVolunteerWriteRepository volunteerRepository,
    ILogger<UpdateSocialNetworkHandler> logger) : ICommandHandler<UpdateSocialNetworksCommand>
{
    private readonly IVolunteerWriteRepository _volunteerRepository = volunteerRepository;
    private readonly ILogger<UpdateSocialNetworkHandler> _logger = logger;
    public async Task<UnitResult> Handle(
        UpdateSocialNetworksCommand command,
        CancellationToken cancelToken)
    {
        var validationResult = ValidateItems(
            command.SocialNetworksDtos,
            (s) => SocialNetworkInfo.Validate(s.Name, s.Url));

        if (validationResult.IsFailure)
        {
            _logger.LogError(
                "UpdateSocialNetworkHandler socials validation failure!{Errors}",
                validationResult.ValidationMessagesToString());

            return validationResult;
        }
        var getVolunteer = await _volunteerRepository.GetByIdAsync(command.volunteerId, cancelToken);
        if (getVolunteer.IsFailure)
            return UnitResult.Fail(getVolunteer.Error);

        var volunteer = getVolunteer.Data!;

        var socialNetworks = command.SocialNetworksDtos
            .Select(social => SocialNetworkInfo.Create(social.Name, social.Url).Data) ?? [];

        await _volunteerRepository.Save(volunteer, cancelToken);

        _logger.LogInformation("Update SocialNetworks for volunteer with id:{Id} successfull.",
            command.volunteerId);

        return UnitResult.Ok();
    }
}
