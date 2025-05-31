using PetFamily.Application.Dtos;
using PetFamily.SharedKernel.Results;
using PetSpecies.Application.Queries.GetBreedPagedList;
using PetSpecies.Application.Queries.GetSpeciesPagedList;
using PetSpecies.Domain;

namespace PetSpecies.Application.IRepositories;

public interface ISpeciesReadRepository
{
    Task<Result<Species>> GetByIdAsync(Guid speciesId, CancellationToken cancelToken = default);

    Task<UnitResult> VerifySpeciesAndBreedExist(
       Guid speciesId,
       Guid breedId,
       CancellationToken cancelToken = default);

    Task<SpeciesPagedListDto> GetSpeciesPagedList(
        SpeciesFilterDto? speciesFilter,
        PaginationDto? pagination,
        CancellationToken cancelToken = default);

    Task<Result<Breed>> GetBreedByIdAsync(Guid breedId, CancellationToken cancelToken = default);

    Task<BreedPagedListDto> GetBreedPagedList(
        BreedFilterDto breedFilter,
        PaginationDto pagination,
        CancellationToken cancelToken = default);

    Task<UnitResult> CheckForDeleteAsync(Guid speciesId, CancellationToken cancelToken = default);

    Task<UnitResult> CheckForDeleteBreedAsync(Guid breedId, CancellationToken cancelToken = default);
}
