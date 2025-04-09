using Amazon.Runtime.Internal.Util;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PetFamily.Application.IRepositories;
using PetFamily.Domain.DomainError;
using PetFamily.Domain.PetTypeManagment.Entities;
using PetFamily.Domain.PetTypeManagment.Root;
using PetFamily.Domain.Results;
using PetFamily.Infrastructure.Contexts;

namespace PetFamily.Infrastructure.Repositories.Write;

public class SpeciesWriteRepository(
    WriteDbContext context,
    ILogger<SpeciesWriteRepository> logger) : ISpeciesWriteRepository
{
    private readonly WriteDbContext _context = context;
    private readonly ILogger<SpeciesWriteRepository> _logger = logger;
    public async Task<Guid> AddAsync(Species species, CancellationToken token)
    {
        await _context.AnimalTypes.AddAsync(species, token);
        await _context.SaveChangesAsync(token);
        return species.Id;
    }

    public async Task<UnitResult> DeleteAsync(Guid speciesId, CancellationToken cancelToken)
    {
        var species = await _context
            .AnimalTypes.FirstOrDefaultAsync(s => s.Id == speciesId, cancelToken);
        if (species == null)
        {
            _logger.LogWarning("Attempted to delete non-existent species with id:{Id}", speciesId);
            return UnitResult.Fail(Error.NotFound($"Species with Id: {speciesId} not found"));
        }
        _context.AnimalTypes.Remove(species);

        await _context.SaveChangesAsync(cancelToken);

        _logger.LogInformation("Successfully deleted species with id: {SpeciesId}", speciesId);

        return UnitResult.Ok();
    }

    public async Task<Result<Breed>> GetBreedByIdAsync(Guid breedId, CancellationToken cancelToken)
    {
        var species = await _context.AnimalTypes
            .Include(s => s.Breeds)
            .FirstOrDefaultAsync(s => s.Breeds.Any(b => b.Id == breedId), cancelToken);
        if (species == null)
        {
            _logger.LogWarning("Attempted to get non-existent breed with id: {BreedId}", breedId);

            return UnitResult.Fail(Error.NotFound($"Breed with Id: {breedId} not found"));
        }

        var breed = species.Breeds.First(b => b.Id == breedId);

        _logger.LogInformation("Successfully retrieved breed with id: {BreedId}", breedId);
        return Result.Ok(breed);
    }

    public Task<Result<List<Breed>>> GetBreedsAsync(Guid speciesId, CancellationToken cancelToken)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<Species>> GetByBreedIdAsync(Guid breedId, CancellationToken cancelToken)
    {
        var species = await _context.AnimalTypes
            .Include(s => s.Breeds)
            .FirstOrDefaultAsync(s => s.Breeds.Any(b => b.Id == breedId), cancelToken);
        if (species == null)
        {
            _logger.LogWarning("Attempted to get species by non-existent breed with id: {BreedId}",
                breedId);

            return UnitResult.Fail(Error.NotFound($"Breed with Id: {breedId} not found"));
        }
        _logger.LogInformation("Successfully retrieved species by breed id:{BreedId}", breedId);

        return Result.Ok(species);
    }

    public async Task<Result<Species>> GetByIdAsync(Guid speciesId, CancellationToken cancelToken)
    {
        var species = await _context.AnimalTypes
             .Include(s => s.Breeds)
             .FirstOrDefaultAsync(s => s.Id == speciesId, cancelToken);
        if (species == null)
        {
            _logger.LogWarning("Attempted to get non-existent species with id:{Id}", speciesId);
            return UnitResult.Fail(Error.NotFound($"Species with Id: {speciesId} not found"));
        }
        _logger.LogInformation("Successfully retrieved species with id: {SpeciesId}", speciesId);
        return UnitResult.Ok(species);
    }

    public Task<Result<List<Species>>> GetSpecies(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task SaveAsync(Species species, CancellationToken token)
    {
        var entries = _context.Entry(species);
        return _context.SaveChangesAsync(token);
    }
}
