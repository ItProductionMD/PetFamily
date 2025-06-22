using Volunteers.Application.Commands.VolunteerManagement.CreateVolunteer;
using Volunteers.Application.ResponseDtos;

namespace Volunteers.Presentation.Requests;

public record CreateVolunteerRequest(
    string FirstName,
    string LastName,
    string Description,
    int ExperienceYears,
    IEnumerable<RequisitesDto> Requisites)
{
    public CreateVolunteerCommand ToCommand() =>
        new(FirstName,
            LastName,
            Description,    
            ExperienceYears,
            Requisites);
}

