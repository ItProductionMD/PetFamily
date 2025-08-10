using PetFamily.SharedKernel.Results;
using PetFamily.VolunteerRequests.Domain.Entities;

namespace PetFamily.VolunteerRequests.Application.IRepositories;

public interface IVolunteerRequestWriteRepository
{
    Task<Result<VolunteerRequest>> GetByIdAsync(Guid volunteerRequestId, CancellationToken ct);
    Task AddAsync(VolunteerRequest volunteerRequest, CancellationToken ct);
    Task SaveAsync(CancellationToken ct);
}
