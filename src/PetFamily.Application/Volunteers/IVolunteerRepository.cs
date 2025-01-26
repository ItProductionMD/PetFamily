using PetFamily.Domain.Shared.DomainResult;
using PetFamily.Domain.Shared.ValueObjects;
using PetFamily.Domain.VolunteerAggregates.Root;

namespace PetFamily.Application.Volunteers;

public interface IVolunteerRepository
{
    Task<Result<Guid>> Add(Volunteer volunteer, CancellationToken cancellation = default);

    Task<Result<List<Volunteer>>> GetByEmailOrPhone(
        string email,
        Phone phone,
        CancellationToken cancellation = default);

    Task<Result> Save(Volunteer volunteer, CancellationToken cancellation = default);

    Task<Result<Volunteer>> GetById(Guid id, CancellationToken cancellation = default);

    Task<Result<Guid>> Delete(Volunteer volunteer, CancellationToken cancellation = default);

    Task<Result> UpdateSocialNetworks(
        Guid volunteerId,
        ValueObjectList<SocialNetwork> socialNetworks,
        CancellationToken cancellation = default);
}
