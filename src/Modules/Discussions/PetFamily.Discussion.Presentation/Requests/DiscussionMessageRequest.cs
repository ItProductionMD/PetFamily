using PetFamily.Discussions.Application.Commands.LeaveMessage;
using PetFamily.Discussions.Application.Commands.UpdateMessage;

namespace PetFamily.Discussion.Presentation.Requests;

public class DiscussionMessageRequest
{
    string Message { get; set; } = string.Empty; // The message text to leave in the discussion

    public LeaveDiscussionMessageCommand ToLeaveCommand(Guid userId,Guid discussionId)
    {
        return new LeaveDiscussionMessageCommand(userId, discussionId, Message);
    }

    public UpdateDiscussionMessageCommand ToUpdateCommand(Guid userId, Guid discussionId, Guid messageId)
    {
        return new UpdateDiscussionMessageCommand(userId, discussionId, messageId, Message);
    }
}
