using PetFamily.Discussions.Public.Contracts;
using PetFamily.SharedKernel.Results;

namespace PetFamily.Discussions.Infrastructure.Contracts;

public class MessageRemover : IMessageRemover
{
    public async Task<UnitResult> RemoveMessage(
        Guid messageId,
        Guid authorId,
        CancellationToken ct = default)
    {
        return UnitResult.Ok();
    }
}
