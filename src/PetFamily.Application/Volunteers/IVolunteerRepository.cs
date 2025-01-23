using PetFamily.Domain.Shared.DomainResult;
using PetFamily.Domain.Shared.ValueObjects;
using PetFamily.Domain.VolunteerAggregates.Root;

namespace PetFamily.Application.Volunteers
{
    public interface IVolunteerRepository
    {
        Task<Result<Guid>> Add(Volunteer volunteer, CancellationToken cancellationToken = default);

        Task<Result<List<Volunteer>>> GetByEmailOrPhone(string email, Phone phone);

    }
}
