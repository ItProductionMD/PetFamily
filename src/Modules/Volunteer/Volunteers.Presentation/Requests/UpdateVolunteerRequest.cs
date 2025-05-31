using Volunteers.Application.Commands.VolunteerManagement.UpdateVolunteer;

namespace Volunteers.Presentation.Requests;

public record UpdateVolunteerRequest
(
    string FirstName,
    string LastName,
    string Email,
    string Description,
    string PhoneNumber,
    string PhoneRegionCode,
    int ExperienceYears
)
{
    public UpdateVolunteerCommand ToCommand(Guid volunteerId) =>
        new UpdateVolunteerCommand(
            volunteerId,
            FirstName,
            LastName,
            Email,
            Description,
            PhoneNumber,
            PhoneRegionCode,
            ExperienceYears);
}

