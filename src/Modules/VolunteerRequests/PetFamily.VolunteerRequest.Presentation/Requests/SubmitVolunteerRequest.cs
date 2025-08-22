using PetFamily.SharedApplication.Dtos;
using PetFamily.VolunteerRequests.Application.Commands.SubmitVolunteerRequest;

namespace PetFamily.VolunteerRequests.Presentation.Requests;

public class SubmitVolunteerRequest
{
    public string DocumentName { get; set; }
    public string LastName { get; set; }
    public string FirstName { get; set; }
    public string Description { get; set; }
    public int ExperienceYears { get; set; }
    public IEnumerable<RequisitesDto> Requisites { get; set; } = [];

    public SubmitVolunteerRequestCommand ToCommand(Guid UserId)
    {
        return new SubmitVolunteerRequestCommand(
            UserId,
            DocumentName,
            LastName,
            FirstName,
            Description,
            ExperienceYears,
            Requisites);
    }
}

