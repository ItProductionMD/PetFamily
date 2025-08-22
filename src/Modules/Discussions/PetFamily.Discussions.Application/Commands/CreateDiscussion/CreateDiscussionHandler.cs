using PetFamily.Discussions.Application.IRepositories;
using PetFamily.Discussions.Domain.Entities;
using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.SharedKernel.Results;

namespace PetFamily.Discussions.Application.Commands.CreateDiscussion;

public class CreateDiscussionHandler(IDiscussionWriteRepository discussionWriteRepo)
    : ICommandHandler<CreateDiscussionCommand>
{
    public async Task<UnitResult> Handle(CreateDiscussionCommand cmd, CancellationToken ct)
    {
        //TODO Validate the command
        var adminId = cmd.AdminId;

        var discussion = Discussion.Create(cmd.VolunteerRequestId, adminId, cmd.UserId).Data!;

        await discussionWriteRepo.AddAndSaveAsync(discussion, ct);

        return UnitResult.Ok();
    }
}
