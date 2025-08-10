using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.Discussions.Application.IRepositories;
using PetFamily.Discussions.Domain.Entities;
using PetFamily.SharedApplication.IUserContext;
using PetFamily.SharedKernel.Results;

namespace PetFamily.Discussions.Application.Commands.CreateDiscussion;

public class CreateDiscussionHandler(
    IDiscussionWriteRepository discussionWriteRepo) : ICommandHandler<CreateDiscussionCommand>
{
    private readonly IDiscussionWriteRepository _discussionWriteRepo = discussionWriteRepo;
    public async Task<UnitResult> Handle(CreateDiscussionCommand cmd, CancellationToken ct)
    {
        //TODO Validate the command
        var adminId = cmd.AdminId;

        var discussion = Discussion.Create(cmd.VolunteerRequestId, adminId, cmd.UserId).Data!;

        await _discussionWriteRepo.AddAndSaveAsync(discussion, ct);

        return UnitResult.Ok();
    }
}
