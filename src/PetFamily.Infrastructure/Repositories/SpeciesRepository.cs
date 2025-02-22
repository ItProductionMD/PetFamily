using Microsoft.EntityFrameworkCore;
using PetFamily.Application.Species;
using PetFamily.Domain.DomainError;
using PetFamily.Domain.PetManagment.Entities;
using PetFamily.Domain.Results;

namespace PetFamily.Infrastructure.Repositories;

public class SpeciesRepository(AppDbContext context) : ISpeciesRepository
{
    private readonly AppDbContext _context = context;
    public async Task<Guid> AddAsync(Species species, CancellationToken token)
    {
        await _context.AnimalTypes.AddAsync(species, token);
        await _context.SaveChangesAsync(token);
        return species.Id;
    }
    public async Task<Species?> GetAsync(Guid speciesId, CancellationToken token)
    {
        return await _context.AnimalTypes
            .Include(s => s.Breeds)
            .FirstOrDefaultAsync(s => s.Id == speciesId, token);
    }

    public Task SaveAsync(Species species, CancellationToken token)
    {
        var entries = _context.Entry(species);
        return _context.SaveChangesAsync(token);
    }
    
}
