using PetFamily.SharedKernel.Results;
using PetFamily.VolunteerRequests.Domain.Entities;

namespace PetFamily.VolunteerRequests.Application.IRepositories;

public interface IVolunteerRequestReadRepository
{
    Task<VolunteerRequest> GetByUserIdAsync(Guid userId, CancellationToken ct);
}
