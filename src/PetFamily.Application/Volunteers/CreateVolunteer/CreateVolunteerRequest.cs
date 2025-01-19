using PetFamily.Domain.Shared.DTO;

namespace PetFamily.Application.Volunteers.CreateVolunteer
{
    public record CreateVolunteerRequest(
        string FirstName,
        string LastName,
        string Email,
        string Description,
        string PhoneNumber,
        string PhoneRegionCode,
        int ExperienceYears,
        IEnumerable<DonateDetailsDTO> DonateDetailsDtos,
        IEnumerable<SocialNetworkDTO> SocialNetworksDtos);
}