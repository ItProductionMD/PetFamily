using PetFamily.SharedKernel.Results;
using PetSpecies.Domain;

namespace PetSpecies.Application.IRepositories;

public interface ISpeciesWriteRepository
{
    Task<Result<Species>> GetByIdAsync(Guid SpeciesId, CancellationToken cancellationToken);

    Task<Result<Species>> GetByBreedIdAsync(Guid breedId, CancellationToken cancellationToken);

    Task<Result<List<Species>>> GetSpecies(CancellationToken cancellationToken);

    Task<Guid> AddAndSaveAsync(Species species, CancellationToken cancellationToken);

    Task SaveAsync(Species species, CancellationToken cancellationToken);

    Task<UnitResult> DeleteAndSaveAsync(Guid speciesId, CancellationToken cancellationToken);

    Task<Result<Breed>> GetBreedByIdAsync(Guid breedId, CancellationToken cancellationToken);

    Task<Result<List<Breed>>> GetBreedsAsync(Guid speciesId, CancellationToken cancellationToken);
}
