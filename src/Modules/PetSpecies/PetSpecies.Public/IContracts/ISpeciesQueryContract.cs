using PetSpecies.Public.Dtos;

namespace PetSpecies.Public.IContracts;

public interface ISpeciesQueryContract
{
    public Task<List<SpeciesDto>> GetAllSpeciesAsync(CancellationToken ct = default);
}
