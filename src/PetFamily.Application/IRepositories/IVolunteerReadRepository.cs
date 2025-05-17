using PetFamily.Application.Dtos;
using PetFamily.Application.Queries.Pet.GetPets;
using PetFamily.Application.Queries.Volunteer.GetVolunteers;
using PetFamily.Domain.Results;

namespace PetFamily.Application.IRepositories;

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

    Task<Result<GetPetsResponse>> GetPets(
        PetsFilter? filter,
        int pageNumber,
        int pageSize,
        CancellationToken cancelToken = default);
}
