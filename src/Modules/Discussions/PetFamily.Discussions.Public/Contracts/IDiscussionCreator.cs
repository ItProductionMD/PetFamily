using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.ValueObjects.Ids;

namespace PetFamily.Discussions.Public.Contracts;

public interface IDiscussionCreator
{
    Task<Result<Guid>> CreateDiscussion(
        Guid volunteerRequestId,
        IEnumerable<Guid> participantIds,
        CancellationToken ct);
}
