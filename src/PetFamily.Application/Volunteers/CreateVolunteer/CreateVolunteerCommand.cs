using PetFamily.Application.Volunteers.Dtos;

namespace PetFamily.Application.Volunteers.CreateVolunteer;

public record CreateVolunteerCommand(
    string FirstName,
    string LastName,
    string Email,
    string Description,
    string PhoneNumber,
    string PhoneRegionCode,
    int ExperienceYears,
    IEnumerable<RequisitesDto> Requisites,
    IEnumerable<SocialNetworksDto> SocialNetworksList);