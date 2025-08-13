using Microsoft.Extensions.Logging;
using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.ValueObjects;
using Volunteers.Application.Commands.PetManagement.UpdateSocials;
using Volunteers.Application.IRepositories;

namespace Volunteers.Application.Commands.PetManagement.UpdateSocialNetworks;

public class UpdateSocialNetworkHandler(
    IVolunteerWriteRepository volunteerWriteRepo,
    ILogger<UpdateSocialNetworkHandler> logger) : ICommandHandler<UpdateSocialNetworksCommand>
{
    public async Task<UnitResult> Handle(UpdateSocialNetworksCommand cmd, CancellationToken ct)
    {
        cmd.Validate();

        var getVolunteer = await volunteerWriteRepo.GetByIdAsync(cmd.volunteerId, ct);
        if (getVolunteer.IsFailure)
            return UnitResult.Fail(getVolunteer.Error);

        var volunteer = getVolunteer.Data!;

        var socialNetworks = cmd.SocialNetworksDtos
            .Select(social => SocialNetworkInfo.Create(social.Name, social.Url).Data) ?? [];

        await volunteerWriteRepo.SaveAsync(volunteer, ct);

        logger.LogInformation("Update SocialNetworks for volunteer with id:{Id} successfull.",
            cmd.volunteerId);

        return UnitResult.Ok();
    }
}
