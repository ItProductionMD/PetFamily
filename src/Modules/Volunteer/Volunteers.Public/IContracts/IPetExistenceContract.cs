namespace Volunteers.Public.IContracts;

public interface IPetExistenceContract
{
    Task<bool> ExistsWithSpeciesAsync(Guid speciesId, CancellationToken ct = default);
    Task<bool> ExistsWithBreedAsync(Guid breedId, CancellationToken ct = default);
}
