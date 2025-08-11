using PetFamily.Discussions.Application.Commands.CreateDiscussion;

namespace PetFamily.Discussions.Presentation.Requests;

public class CreateDiscussionRequest
{
    public Guid UserId { get; set; }
    public Guid VolunteerRequestId { get; set; }
    public CreateDiscussionCommand ToCommand(Guid adminId)
    {
        return new CreateDiscussionCommand(adminId, UserId, VolunteerRequestId);
    }
}
