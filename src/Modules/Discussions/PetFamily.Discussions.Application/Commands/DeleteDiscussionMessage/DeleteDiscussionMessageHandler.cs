using Microsoft.Extensions.Logging;
using PetFamily.Discussions.Application.IRepositories;
using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.SharedKernel.Results;

namespace PetFamily.Discussions.Application.Commands.DeleteDiscussionMessage;

public class DeleteDiscussionMessageHandler(
    IDiscussionWriteRepository discussionWriteRepo,
    ILogger<DeleteDiscussionMessageHandler> logger) : ICommandHandler<DeleteDiscussionMessageCommand>
{

    public async Task<UnitResult> Handle(DeleteDiscussionMessageCommand cmd, CancellationToken ct)
    {
        var authorId = cmd.UserId;

        var getDiscussion = await discussionWriteRepo.GetById(cmd.DiscussionId, ct);

        if (getDiscussion.IsFailure)
            return UnitResult.Fail(getDiscussion.Error);

        var discussion = getDiscussion.Data!;

        var deleteMessageResult = discussion.DeleteMessage(authorId, cmd.MessageId);
        if (deleteMessageResult.IsFailure)
        {
            logger.LogError("Failed to delete message in discussion with id:{DiscussionId}. Error: {Error}",
                cmd.DiscussionId, deleteMessageResult.ToErrorMessage());
            return UnitResult.Fail(deleteMessageResult.Error);
        }
        logger.LogInformation("Message with id:{MessageId} deleted in discussion with id:{DiscussionId} by user:{UserId}",
            cmd.MessageId, cmd.DiscussionId, authorId);

        await discussionWriteRepo.SaveAsync(ct);

        return UnitResult.Ok();
    }
}
