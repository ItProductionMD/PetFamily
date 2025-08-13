using PetFamily.Auth.Public.Contracts;
using PetFamily.Discussions.Public.Dtos;
using PetFamily.SharedKernel.Results;

namespace Volunteers.Application.Contracts;

public class ParticipantContract : IParticipantContract
{
    public Task<Result<List<ParticipantDto>>> GetByIds(IReadOnlyList<Guid> guids, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
