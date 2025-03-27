using PetFamily.Application.Abstractions;
using PetFamily.Application.Dtos;

namespace PetFamily.Application.Commands.PetManagment.UpdateSocialNetworks;

public record UpdateSocialNetworksCommand(
    Guid volunteerId, 
    IEnumerable<SocialNetworksDto> SocialNetworksDtos) : ICommand;
