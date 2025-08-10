using PetFamily.SharedKernel.Results;

namespace PetFamily.Discussions.Public.Contracts;

public interface IMessageRemover
{
    Task<UnitResult> RemoveMessage(Guid messageId, Guid authorId, CancellationToken ct = default);
}
