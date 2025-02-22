using Microsoft.EntityFrameworkCore;
using PetFamily.Application.Volunteers;
using PetFamily.Domain.DomainError;
using PetFamily.Domain.PetManagment.Entities;
using PetFamily.Domain.PetManagment.Root;
using PetFamily.Domain.Results;
using PetFamily.Domain.Shared.ValueObjects;
using System.Runtime.CompilerServices;
using System.Threading;

namespace PetFamily.Infrastructure.Repositories;

public class VolunteerRepository : IVolunteerRepository
{
    private readonly AppDbContext _context;

    public VolunteerRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task Add(
        Volunteer volunteer,
        CancellationToken cancellationToken = default)
    {
        await _context.Volunteers.AddAsync(volunteer, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result<List<Volunteer>>> GetByEmailOrPhone(
        string email,
        Phone phone,
        CancellationToken cancellation = default)
    {
        List<Volunteer> volunteers = await _context.Volunteers
            .Where(v => v.Email == email ||
            v.PhoneNumber.RegionCode == phone.RegionCode &&
            v.PhoneNumber.Number == phone.Number).ToListAsync(cancellation);

        if (volunteers.Count == 0)
            return Result.Fail(Error.NotFound("Volunteers"));

        return Result.Ok(volunteers);
    }

    public async Task<Result<Volunteer>> GetByIdAsync(Guid id, CancellationToken cancellation = default)
    {
        var volunteer = await _context.Volunteers
            .Include(v => v.Pets)
            .FirstOrDefaultAsync(v => v.Id == id, cancellation);
        if (volunteer == null)
            return Result.Fail(Error.NotFound("Volunteer"));

        return Result.Ok(volunteer);
    }

    public async Task Save(Volunteer volunteer, CancellationToken cancellToken = default)
    {
        await _context.SaveChangesAsync(cancellToken);
    }
    public  void  SetPetStateAdded(Pet pet)
    {
       _context.Entry(pet).State = EntityState.Added;
    }
    public async Task Delete(
        Volunteer volunteer,
        CancellationToken cancellToken = default)
    {
        _context.Volunteers.Remove(volunteer);
        await _context.SaveChangesAsync(cancellToken);
    }
    public async Task<UnitResult> UpdateSocialNetworks(
        Guid volunteerId,
        List<SocialNetworkInfo> socialNetworks,
        CancellationToken cancellation = default)
    {
        var socialNetworksValue = await _context.Volunteers
             .Where(v => v.Id == volunteerId)
             .Select(v => v.SocialNetworks)
             .FirstOrDefaultAsync(cancellation);

        if (socialNetworksValue == null)
            return UnitResult.Fail(Error.NotFound("Volunteer"));

        if(socialNetworksValue == socialNetworks)
            return UnitResult.Ok();

        await _context.SaveChangesAsync(cancellation);

        return UnitResult.Ok();
    }

}