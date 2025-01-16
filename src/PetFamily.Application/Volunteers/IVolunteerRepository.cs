using PetFamily.Domain.Shared.DomainResult;
using PetFamily.Domain.VolunteerAggregates.Root;

namespace PetFamily.Application.Volunteers
{
    public interface IVolunteerRepository
    {
        Task<Result<Guid>> Add(Volunteer volunteer, CancellationToken cancellationToken = default);

    }
}
