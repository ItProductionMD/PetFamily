using PetFamily.SharedKernel.Results;
using Volunteers.Application.Queries.GetPets;
using Volunteers.Application.Queries.GetPets.ForFilter;
using Volunteers.Application.Queries.GetVolunteers;
using Volunteers.Application.ResponseDtos;

namespace Volunteers.Application.IRepositories;

public interface IVolunteerReadRepository
{
    Task<GetVolunteersResponse> GetVolunteers(
        GetVolunteersQuery query,
        CancellationToken cancelToken = default);

    Task<Result<VolunteerDto>> GetByIdAsync(
        Guid volunteerId,
        CancellationToken cancelToken = default);

    Task<UnitResult> CheckUniqueFields(
        Guid volunteerId,
        string phoneRegionCode,
        string phoneNuber,
        string email,
        CancellationToken cancelToken = default);

    Task<Result<GetPetsResponse>> GetPetPagedList(
        PetsFilter? filter,
        int pageNumber,
        int pageSize,
        CancellationToken cancelToken = default);
}
