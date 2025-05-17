using PetFamily.Application.Dtos;
using PetFamily.Application.Queries.PetType.GetBreeds;
using PetFamily.Application.Queries.PetType.GetListOfSpecies;
using PetFamily.Domain.PetTypeManagment.Entities;
using PetFamily.Domain.PetTypeManagment.Root;
using PetFamily.Domain.Results;

namespace PetFamily.Application.IRepositories;

public interface ISpeciesReadRepository
{
    Task<Result<Species>> GetByIdAsync(Guid speciesId, CancellationToken cancelToken = default);

    Task<UnitResult> CheckIfPetTypeExists(
       Guid speciesId,
       Guid breedId,
       CancellationToken cancelToken = default);

    Task<GetListOfSpeciesResponse> GetListOfSpeciesAsync(
        GetListOfSpeciesQuery pagination,
        CancellationToken cancelToken = default);

    Task<Result<Breed>> GetBreedByIdAsync(Guid breedId, CancellationToken cancelToken = default);

    Task<GetBreedsResponse> GetBreedsAsync(GetBreedsQuery query, CancellationToken cancelToken = default);

    Task<UnitResult> CheckForDeleteAsync(Guid speciesId, CancellationToken cancelToken = default);

    Task<UnitResult> CheckForDeleteBreedAsync(Guid breedId, CancellationToken cancelToken = default);
    Task<Result<List<SpeciesDto>>> GetSpeciesDtos(CancellationToken cancellationToken = default);

}
