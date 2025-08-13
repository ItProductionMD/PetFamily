using PetFamily.VolunteerRequests.Application.Commands.RejectVolunteerRequest;

namespace PetFamily.VolunteerRequests.Presentation.Requests;

public class RejectVolunteerRequest
{
    public string Message { get; set; } = string.Empty;

    public RejectVolunteerRequestCommand ToCommand(Guid adminId, Guid volunteerRequestId)
    {
        return new RejectVolunteerRequestCommand(adminId, volunteerRequestId, Message);
    }
}
