using PetSpecies = PetFamily.Domain.PetManagment.Entities.Species;
using PetFamily.Domain.Results;

namespace PetFamily.Application.Species;

public interface ISpeciesRepository
{
    Task<Guid> AddAsync(PetSpecies species,CancellationToken cancellationToken);
    Task SaveAsync(PetSpecies species,CancellationToken cancellationToken);
    Task<PetSpecies?> GetAsync(Guid id,CancellationToken cancellationToken);
}
