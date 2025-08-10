using Microsoft.Extensions.Logging;
using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.Discussions.Application.IRepositories;
using PetFamily.SharedApplication.IUserContext;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;

namespace PetFamily.Discussions.Application.Commands.CloseDiscussion;

public class CloseDiscussionHandler(
    IDiscussionWriteRepository discussionWriteRepo,
    ILogger<CloseDiscussionHandler> logger) : ICommandHandler<CloseDiscussionCommand>
{
    private readonly IDiscussionWriteRepository _discussionWriteRepo = discussionWriteRepo;
    private readonly ILogger<CloseDiscussionHandler> _logger = logger;
    public async Task<UnitResult> Handle(CloseDiscussionCommand cmd, CancellationToken ct)
    {
        //TODO Validate the command
        var adminId = cmd.AdminId;

        var getDiscussion = await _discussionWriteRepo.GetById(cmd.DiscussionId, ct);
        if(getDiscussion.IsFailure)
            return UnitResult.Fail(getDiscussion.Error);
        
        var discussion = getDiscussion.Data!;  

        var closeResult = discussion.Close(adminId);

        if (closeResult.IsFailure)
        {
            _logger.LogWarning("Failed to close discussion with id:{DiscussionId}. Error: {Error}",
                cmd.DiscussionId, closeResult.ToErrorMessage());
            return UnitResult.Fail(closeResult.Error);
        }
        _logger.LogInformation("Discussion with id:{DiscussionId} closed successfully by admin:{AdminId}",
            cmd.DiscussionId, adminId);

        await _discussionWriteRepo.SaveAsync(ct);

        return UnitResult.Ok();
    }
}
