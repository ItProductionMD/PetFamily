using PetFamily.Application.Abstractions.CQRS;
using Volunteers.Application.ResponseDtos;

namespace Volunteers.Application.Commands.PetManagement.UpdateSocialNetworks;

public record UpdateSocialNetworksCommand(
    Guid volunteerId,
    IEnumerable<SocialNetworksDto> SocialNetworksDtos) : ICommand;
