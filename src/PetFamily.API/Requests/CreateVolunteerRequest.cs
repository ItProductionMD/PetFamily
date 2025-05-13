using PetFamily.Application.Commands.VolunteerManagment.CreateVolunteer;
using PetFamily.Application.Dtos;

namespace PetFamily.API.Requests;

public record CreateVolunteerRequest (
    string FirstName,
    string LastName,
    string Email,
    string Description,
    string PhoneNumber,
    string PhoneRegionCode,
    int ExperienceYears,
    IEnumerable<RequisitesDto> Requisites,
    IEnumerable<SocialNetworksDto> SocialNetworksList
)
{
    public CreateVolunteerCommand ToCommand() => 
        new(FirstName, 
            LastName, 
            Email, 
            Description, 
            PhoneNumber, 
            PhoneRegionCode,
            ExperienceYears,
            Requisites, 
            SocialNetworksList);
}

