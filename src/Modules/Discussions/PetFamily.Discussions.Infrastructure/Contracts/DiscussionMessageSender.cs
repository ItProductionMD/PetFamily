using PetFamily.Discussions.Public.Contracts;
using PetFamily.SharedKernel.Results;

namespace PetFamily.Discussions.Infrastructure.Contracts;

public class DiscussionMessageSender : IDiscussionMessageSender
{
    public async Task<Result<Guid>> Send(
        Guid relationId,
        Guid fromUserId,
        string message,
        CancellationToken ct)
    {
        return Result.Ok(Guid.NewGuid());
    }
}
