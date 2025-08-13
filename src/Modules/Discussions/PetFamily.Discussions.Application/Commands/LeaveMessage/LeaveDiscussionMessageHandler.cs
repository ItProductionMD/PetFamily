using Microsoft.Extensions.Logging;
using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.Discussions.Application.IRepositories;
using PetFamily.SharedApplication.IUserContext;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using System.ComponentModel.DataAnnotations;

namespace PetFamily.Discussions.Application.Commands.LeaveMessage;

public class LeaveDiscussionMessageHandler(
    IDiscussionWriteRepository duscussionWriteRepo,
    ILogger<LeaveDiscussionMessageHandler> logger): ICommandHandler<LeaveDiscussionMessageCommand>
{
    private readonly IDiscussionWriteRepository _discussionWriteRepo = duscussionWriteRepo;
    private readonly ILogger<LeaveDiscussionMessageHandler> _logger = logger;
    public async Task<UnitResult> Handle(LeaveDiscussionMessageCommand cmd, CancellationToken ct)
    {
        var authorId = cmd.UserId;

        var getDiscussion = await _discussionWriteRepo.GetById(cmd.DiscussionId, ct);
        if(getDiscussion.IsFailure)
            return UnitResult.Fail(getDiscussion.Error);

        var discussion = getDiscussion.Data!;   

        var leaveMessageResult = discussion.LeaveMessage(authorId, cmd.Message);
        if(leaveMessageResult.IsFailure)
        {
            _logger.LogError("Failed to leave message in discussion with id:{DiscussionId}. Error: {Error}",
                cmd.DiscussionId, leaveMessageResult.ToErrorMessage());

            return UnitResult.Fail(leaveMessageResult.Error);
        }

        _logger.LogInformation("Message left in discussion with id:{DiscussionId} by user:{UserId}",
            cmd.DiscussionId, authorId);

        await _discussionWriteRepo.SaveAsync(ct);

        return UnitResult.Ok();
    }
}
