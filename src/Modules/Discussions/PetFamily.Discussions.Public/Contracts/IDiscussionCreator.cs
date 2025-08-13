using PetFamily.SharedKernel.Results;

namespace PetFamily.Discussions.Public.Contracts;

public interface IDiscussionCreator
{
    Task<Result<Guid>> CreateDiscussion(
        Guid volunteerRequestId,
        Guid adminId,
        Guid userId,
        CancellationToken ct);
}
