using PetFamily.Discussions.Application.Commands.LeaveMessage;
using PetFamily.Discussions.Application.Commands.UpdateMessage;

namespace PetFamily.Discussions.Presentation.Requests;

public class DiscussionMessageRequest
{
    public string Message { get; set; }

    public LeaveDiscussionMessageCommand ToLeaveCommand(Guid userId, Guid discussionId)
    {
        return new LeaveDiscussionMessageCommand(userId, discussionId, Message);
    }

    public UpdateDiscussionMessageCommand ToUpdateCommand(Guid userId, Guid discussionId, Guid messageId)
    {
        return new UpdateDiscussionMessageCommand(userId, discussionId, messageId, Message);
    }
}
