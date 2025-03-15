using PetFamily.Application.Dtos;
using PetFamily.Application.Queries;

namespace PetFamily.Application.IRepositories;

public interface IVolunteerReadRepository
{
    Task<GetVolunteersResponse> GetVolunteers(
        GetVolunteersQuery query,
        CancellationToken cancellToken = default);
}
