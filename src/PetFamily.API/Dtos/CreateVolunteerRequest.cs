using PetFamily.Application.Commands.PetManagment.Dtos;
using PetFamily.Application.Commands.VolunteerManagment.CreateVolunteer;

namespace PetFamily.API.Dtos;

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

