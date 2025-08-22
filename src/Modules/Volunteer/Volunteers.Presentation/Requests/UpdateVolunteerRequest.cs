using Volunteers.Application.Commands.VolunteerManagement.UpdateVolunteer;

namespace Volunteers.Presentation.Requests;

public class UpdateVolunteerRequest
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Description { get; set; }

    public string PhoneRegionCode { get; set; }
    public int ExperienceYears { get; set; }


    public UpdateVolunteerCommand ToCommand(Guid UserId, Guid volunteerId) =>
        new UpdateVolunteerCommand(
            UserId,
            volunteerId,
            FirstName,
            LastName,
            Description,
            ExperienceYears);
}

