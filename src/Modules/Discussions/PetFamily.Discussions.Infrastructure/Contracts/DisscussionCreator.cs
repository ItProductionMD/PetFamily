using PetFamily.Discussions.Application.Commands.CreateDiscussion;
using PetFamily.Discussions.Public.Contracts;
using PetFamily.SharedKernel.Results;

namespace PetFamily.Discussions.Infrastructure.Contracts;

public class DisscussionCreator(CreateDiscussionHandler handler) : IDiscussionCreator
{
    public async Task<Result<Guid>> CreateDiscussion(
        Guid volunteerRequestId,
        Guid adminId,
        Guid userId,
        CancellationToken ct)
    {
        var command = new CreateDiscussionCommand(adminId, userId, volunteerRequestId);

        var result = await handler.Handle(command, ct);

        return result;
    }
}
