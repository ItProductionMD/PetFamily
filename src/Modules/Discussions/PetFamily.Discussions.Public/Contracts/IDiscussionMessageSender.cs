
using PetFamily.SharedKernel.Results;

namespace PetFamily.Discussions.Public.Contracts;

public interface IDiscussionMessageSender
{
    Task<Result<Guid>> Send(
        Guid relationId,
        Guid fromUserId,
        string message, 
        CancellationToken ct);
}
