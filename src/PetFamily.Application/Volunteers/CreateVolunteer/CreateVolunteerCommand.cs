using static PetFamily.Application.Volunteers.SharedVolunteerRequests;

namespace PetFamily.Application.Volunteers.CreateVolunteer
{
    public record CreateVolunteerCommand(
        string FirstName,
        string LastName,
        string Email,
        string Description,
        string PhoneNumber,
        string PhoneRegionCode,
        int ExperienceYears,
        IEnumerable<DonateDetailsRequest> DonateDetailsList,
        IEnumerable<SocialNetworksRequest> SocialNetworksList);
}