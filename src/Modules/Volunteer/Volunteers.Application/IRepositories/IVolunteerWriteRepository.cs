using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.ValueObjects;
using VolunteerFromDomain = Volunteers.Domain.Volunteer;

namespace Volunteers.Application.IRepositories;

public interface IVolunteerWriteRepository
{
    Task<Result<Guid>> AddAsync(VolunteerFromDomain volunteer, CancellationToken cancellation = default);

    Task<UnitResult> Save(VolunteerFromDomain volunteer, CancellationToken cancelToken = default);

    Task SaveWithRetry(VolunteerFromDomain volunteer, CancellationToken cancelToken = default);

    Task<Result<VolunteerFromDomain>> GetByIdAsync(Guid id, CancellationToken cancelToken = default);

    Task Delete(VolunteerFromDomain volunteer, CancellationToken cancelToken = default);
}
