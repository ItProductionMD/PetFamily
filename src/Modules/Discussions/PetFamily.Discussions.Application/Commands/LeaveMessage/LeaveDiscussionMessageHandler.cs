using Microsoft.Extensions.Logging;
using PetFamily.Discussions.Application.IRepositories;
using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.SharedKernel.Results;

namespace PetFamily.Discussions.Application.Commands.LeaveMessage;

public class LeaveDiscussionMessageHandler(
    IDiscussionWriteRepository discussionWriteRepo,
    ILogger<LeaveDiscussionMessageHandler> logger) : ICommandHandler<LeaveDiscussionMessageCommand>
{
    public async Task<UnitResult> Handle(LeaveDiscussionMessageCommand cmd, CancellationToken ct)
    {
        var authorId = cmd.UserId;

        var getDiscussion = await discussionWriteRepo.GetById(cmd.DiscussionId, ct);
        if (getDiscussion.IsFailure)
            return UnitResult.Fail(getDiscussion.Error);

        var discussion = getDiscussion.Data!;

        var leaveMessageResult = discussion.LeaveMessage(authorId, cmd.Message);
        if (leaveMessageResult.IsFailure)
        {
            logger.LogError("Failed to leave message in discussion with id:{DiscussionId}. Error: {Error}",
                cmd.DiscussionId, leaveMessageResult.ToErrorMessage());

            return UnitResult.Fail(leaveMessageResult.Error);
        }

        logger.LogInformation("Message left in discussion with id:{DiscussionId} by user:{UserId}",
            cmd.DiscussionId, authorId);

        await discussionWriteRepo.SaveAsync(ct);

        return UnitResult.Ok();
    }
}
