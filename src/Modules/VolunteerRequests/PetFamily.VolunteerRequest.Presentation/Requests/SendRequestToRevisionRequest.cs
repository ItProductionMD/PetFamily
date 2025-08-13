using PetFamily.VolunteerRequests.Application.Commands.RejectVolunteerRequest;
using PetFamily.VolunteerRequests.Application.Commands.SendVolunteerRequestToRevision;

namespace PetFamily.VolunteerRequests.Presentation.Requests;

public class SendRequestToRevisionRequest
{
    public string Message { get; set; } = string.Empty;

    public SendRequestToRevisionCommand ToCommand(Guid adminId, Guid volunteerRequestId)
    {
        return new SendRequestToRevisionCommand(adminId, volunteerRequestId, Message);
    }
}
