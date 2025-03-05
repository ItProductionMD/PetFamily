using PetFamily.Application.Volunteers.Dtos;

namespace PetFamily.Application.Volunteers.UpdateSocialNetworks;

public record UpdateSocialNetworksCommand(
    Guid volunteerId, 
    IEnumerable<SocialNetworksDto> SocialNetworksDtos);
