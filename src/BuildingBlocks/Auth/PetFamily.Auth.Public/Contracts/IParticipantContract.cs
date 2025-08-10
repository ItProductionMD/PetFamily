using PetFamily.Discussions.Public.Dtos;
using PetFamily.SharedKernel.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetFamily.Auth.Public.Contracts;

public interface IParticipantContract
{
    Task<Result<List<ParticipantDto>>> GetByIds(IReadOnlyList<Guid> guids, CancellationToken ct);
}
