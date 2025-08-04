using PetFamily.SharedApplication.Dtos;
using Volunteers.Application.Commands.VolunteerManagement.CreateVolunteer;
using Volunteers.Application.ResponseDtos;

namespace Volunteers.Presentation.Requests;

public record CreateVolunteerRequest(
    string FirstName,
    string LastName,
    string Description,
    int ExperienceYears,
    string PhoneRegionCode,
    string PhoneNumber,
    IEnumerable<RequisitesDto> Requisites)
{
    public CreateVolunteerCommand ToCommand() =>
        new(
            Guid.NewGuid(),
            FirstName,
            LastName,
            Description,    
            ExperienceYears,
            PhoneRegionCode,
            PhoneNumber,
            Requisites);
}

