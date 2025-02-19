
using Microsoft.EntityFrameworkCore;
using PetFamily.Application.Pets;
using PetFamily.Domain.DomainError;
using PetFamily.Domain.PetAggregates.Root;
using PetFamily.Domain.Results;
using PetFamily.Domain.VolunteerAggregates.Root;

namespace PetFamily.Infrastructure.Repositories;

public class PetRepository(AppDbContext context) : IPetRepository
{
    private readonly AppDbContext _context = context;
    public async Task<UnitResult> AddAsync(Volunteer volunteer, Pet newPet)
    {
        try
        {
            _context.Add(newPet);
            await _context.SaveChangesAsync();
            return UnitResult.Ok();
        }
        catch
        {
            return UnitResult.Fail(Error.InternalServerError("Add pet id DB failed!"));
        }
    }

    public async Task<Result<Pet>> GetAsync(Guid petId,CancellationToken token)
    {
        var pet = await _context.Volunteers
            .Where(v => v.Pets.Any(p => p.Id == petId))
            .SelectMany(v => v.Pets)
            .FirstOrDefaultAsync(p => p.Id == petId,token);
        if (pet == null)
            return Result.Fail(Error.NotFound("Pet"));

        return Result.Ok(pet);
    }

    public async Task<UnitResult> UpdateImages(Pet pet,CancellationToken cancellationToken)
    {
        try
        {
            _context.Attach(pet);
            _context.Entry(pet).Property(p => p.Images).IsModified = true;
            await _context.SaveChangesAsync(cancellationToken);
            return UnitResult.Ok();
        }
        catch(Exception ex)
        {
            return UnitResult.Fail(Error.Exception(ex));
        }
    }
}
