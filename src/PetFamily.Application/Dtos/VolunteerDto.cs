namespace PetFamily.Application.Dtos;

public class VolunteerDto
{
    public Guid Id { get; set; }
    public string LastName { get; set; }
    public string FirstName { get; set; }
    public string PhoneRegionCode { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public IReadOnlyList<SocialNetworksDto> SocialNetworkDtos { get; set; }
    public IReadOnlyList<RequisitesDto> RequisitesDtos { get; set; }
    public List<PetMainInfoDto> PetDtos { get; set; } = [];

    public VolunteerDto(
        Guid id,
        string lastName,
        string firstName,
        string phoneRegionCode,
        string phoneNumber,
        string email,
        List<SocialNetworksDto> socialNetworkDtos,
        List<RequisitesDto> requisitesDtos,
        List<PetMainInfoDto>? petDtos)
    {
        Id = id;
        LastName = lastName;
        FirstName = firstName;
        PhoneRegionCode = phoneRegionCode;
        PhoneNumber = phoneNumber;
        Email = email;
        SocialNetworkDtos = socialNetworkDtos;
        RequisitesDtos = requisitesDtos;
        PetDtos = petDtos ?? [];
    }
}