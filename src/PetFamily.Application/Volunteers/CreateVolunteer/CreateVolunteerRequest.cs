namespace PetFamily.Application.Volunteers.CreateVolunteer
{
    public record CreateVolunteerRequest(
        string FirstName,
        string LastName,
        string Email,
        string Description,
        string PhoneNumber,
        string PhoneRegionCode,
        int ExpirienceYears,
        List<DonateDetailsDto> DonateDetailsDtos,
        List<SocialNetworksDto> SocialNetworksDtos);
}