using PetFamily.SharedApplication.Dtos;
using PetFamily.VolunteerRequests.Application.Commands.UpdateVolunteerRequest;

namespace PetFamily.VolunteerRequests.Presentation.Requests;

public class UpdateVolunteerRequestRequest
{
    public string LastName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string DocumentName { get; set; } = string.Empty;
    public int ExperienceYears { get; set; }
    public IEnumerable<RequisitesDto> Requisites { get; set; } = new List<RequisitesDto>();

    public UpdateVolunteerRequestCommand ToCommand(Guid userId, Guid requestId)
    {
        return new UpdateVolunteerRequestCommand(
            userId,
            requestId,
            LastName,
            FirstName,
            Description,
            DocumentName,
            ExperienceYears,
            Requisites);
    }
}
