using PetFamily.SharedKernel.Results;

namespace PetSpecies.Public.IContracts;

public interface ISpeciesExistenceContract
{
    Task<UnitResult> VerifySpeciesAndBreedExist(
       Guid speciesId,
       Guid breedId,
       CancellationToken cancelToken = default);
}
