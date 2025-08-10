using PetFamily.Discussions.Public.Contracts;
using PetFamily.SharedKernel.Results;

namespace PetFamily.Discussions.Infrastructure.Contracts;

public class DisscussionCreator : IDiscussionCreator
{
    public async Task<Result<Guid>> CreateDiscussion(
        Guid volunteerRequestId, 
        IEnumerable<Guid> participantIds, 
        CancellationToken ct)
    {
        return Result.Ok(Guid.NewGuid());
    }
}
