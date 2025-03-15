using PetFamily.Application.Commands.PetManagment.Dtos;

namespace PetFamily.Application.Commands.PetManagment.UpdateSocialNetworks;

public record UpdateSocialNetworksCommand(
    Guid volunteerId, 
    IEnumerable<SocialNetworksDto> SocialNetworksDtos);
