using Microsoft.Extensions.Logging;
using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.Discussions.Application.IRepositories;
using PetFamily.SharedApplication.IUserContext;
using PetFamily.SharedKernel.Results;

namespace PetFamily.Discussions.Application.Commands.UpdateMessage;

public class UpdateDiscussionMessageHandler(
    IDiscussionWriteRepository discussionWriteRepo,
    ILogger<UpdateDiscussionMessageHandler> logger) : ICommandHandler<UpdateDiscussionMessageCommand>
{
    private readonly IDiscussionWriteRepository _discussionWriteRepo = discussionWriteRepo;
    private readonly ILogger<UpdateDiscussionMessageHandler> _logger = logger;

    public async Task<UnitResult> Handle(UpdateDiscussionMessageCommand cmd, CancellationToken ct)
    {
        var authorId = cmd.UserId;
        
        var getDiscussion = await _discussionWriteRepo.GetById(cmd.DiscussionId, ct);
        if (getDiscussion.IsFailure)
            return UnitResult.Fail(getDiscussion.Error);

        var discussion = getDiscussion.Data!;

        var editMessageResult = discussion.EditMessage(authorId, cmd.MessageId, cmd.NewMessage);
        if (editMessageResult.IsFailure)
        {
            _logger.LogError("Failed to update message in discussion with id:{DiscussionId}. Error: {Error}",
                cmd.DiscussionId, editMessageResult.ToErrorMessage());
            return UnitResult.Fail(editMessageResult.Error);
        }
        await _discussionWriteRepo.SaveAsync(ct);

        _logger.LogInformation("Message with id:{MessageId} updated in discussion with id:{DiscussionId} by user:{UserId}",
            cmd.MessageId, cmd.DiscussionId, authorId);

        return UnitResult.Ok();
    }
}
