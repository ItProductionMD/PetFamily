using PetFamily.Domain.PetManagment.Entities;
using PetFamily.Domain.PetManagment.Root;
using PetFamily.Domain.Results;
using PetFamily.Domain.Shared.ValueObjects;

namespace PetFamily.Application.IRepositories;

public interface IVolunteerWriteRepository
{
    Task<Result<Guid>> AddAsync(Volunteer volunteer, CancellationToken cancellation = default);

    Task<UnitResult> Save(Volunteer volunteer, CancellationToken cancelToken = default);

    Task SaveWithRetry(Volunteer volunteer, CancellationToken cancelToken = default);

    Task <Result<Volunteer>> GetByIdAsync(Guid id, CancellationToken cancelToken = default);

    Task Delete(Volunteer volunteer, CancellationToken cancelToken = default);

    Task<UnitResult> UpdateSocialNetworks(
        Guid volunteerId,
        List<SocialNetworkInfo> socialNetworks,
        CancellationToken cancelToken = default);
}
