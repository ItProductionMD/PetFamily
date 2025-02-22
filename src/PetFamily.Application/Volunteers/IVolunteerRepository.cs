using PetFamily.Domain.PetManagment.Root;
using PetFamily.Domain.Results;
using PetFamily.Domain.Shared.ValueObjects;

namespace PetFamily.Application.Volunteers;

public interface IVolunteerRepository
{
    Task Add(Volunteer volunteer, CancellationToken cancellation = default);

    Task<Result<List<Volunteer>>> GetByEmailOrPhone(
        string email,
        Phone phone,
        CancellationToken cancellation = default);

    Task Save(Volunteer volunteer, CancellationToken cancellation = default);

    Task<Result<Volunteer>> GetByIdAsync(Guid id, CancellationToken cancellation = default);

    Task Delete(Volunteer volunteer, CancellationToken cancellation = default);

    Task<UnitResult> UpdateSocialNetworks(
        Guid volunteerId,
        List<SocialNetworkInfo> socialNetworks,
        CancellationToken cancellation = default);
}
