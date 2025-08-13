using PetFamily.SharedKernel.Results;
using Volunteers.Domain;

namespace Volunteers.Application.IRepositories;

public interface IVolunteerWriteRepository
{
    Task<Result<Volunteer>> GetByUserIdAsync(Guid userId, CancellationToken cancelToken = default);

    Task<Result<Guid>> AddAndSaveAsync(Volunteer volunteer, CancellationToken cancellation = default);

    Task<UnitResult> SaveAsync(Volunteer volunteer, CancellationToken cancelToken = default);

    Task SaveWithRetry(Volunteer volunteer, CancellationToken cancelToken = default);

    Task<Result<Volunteer>> GetByIdAsync(Guid id, CancellationToken cancelToken = default);

    Task Delete(Volunteer volunteer, CancellationToken cancelToken = default);
}
