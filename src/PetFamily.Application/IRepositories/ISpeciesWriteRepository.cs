using PetFamily.Domain.PetTypeManagment.Entities;
using PetFamily.Domain.Results;
using PetSpecies = PetFamily.Domain.PetTypeManagment.Root.Species;

namespace PetFamily.Application.IRepositories;

public interface ISpeciesWriteRepository
{
    Task<Result<PetSpecies>> GetByIdAsync(Guid SpeciesId, CancellationToken cancellationToken);

    Task<Result<PetSpecies>> GetByBreedIdAsync(Guid breedId, CancellationToken cancellationToken);

    Task<Result<List<PetSpecies>>> GetSpecies(CancellationToken cancellationToken);

    Task<Guid> AddAsync(PetSpecies species, CancellationToken cancellationToken);

    Task SaveAsync(PetSpecies species, CancellationToken cancellationToken);

    Task<UnitResult> DeleteAsync(Guid speciesId, CancellationToken cancellationToken);

    Task<Result<Breed>> GetBreedByIdAsync(Guid breedId, CancellationToken cancellationToken);

    Task<Result<List<Breed>>> GetBreedsAsync(Guid speciesId, CancellationToken cancellationToken);
}
