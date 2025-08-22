using PetFamily.Discussions.Public.Dtos;
using PetFamily.SharedKernel.Results;

namespace Account.Public.Contracts;

public interface IParticipantContract
{
    Task<Result<List<ParticipantDto>>> GetByIds(IReadOnlyList<Guid> guids, CancellationToken ct);
}
