using Microsoft.EntityFrameworkCore;
using PetFamily.Application.Volunteers;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.DomainResult;
using PetFamily.Domain.Shared.ValueObjects;
using PetFamily.Domain.VolunteerAggregates.Root;
using System.Threading;

namespace PetFamily.Infrastructure.Repositories;

public class VolunteerRepository : IVolunteerRepository
{
    private readonly AppDbContext _context;

    public VolunteerRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Add(
        Volunteer volunteer,
        CancellationToken cancellationToken = default)
    {
        await _context.Volunteers.AddAsync(volunteer, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(volunteer.Id);
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
            return Result<List<Volunteer>>.Failure(Error.CreateErrorNotFound("Volunteers"));

        return Result<List<Volunteer>>.Success(volunteers);
    }

    public async Task<Result<Volunteer>> GetById(Guid id, CancellationToken cancellation = default)
    {
        var volunteer = await _context.Volunteers
            .FirstOrDefaultAsync(v => v.Id == id, cancellation);

        if (volunteer == null)
            return Result<Volunteer>.Failure(Error.CreateErrorNotFound("Volunteer"));

        return Result<Volunteer>.Success(volunteer);
    }

    public async Task<Result> Save(Volunteer volunteer, CancellationToken cancellToken = default)
    {
        var entries = _context.Entry(volunteer);

        await _context.SaveChangesAsync(cancellToken);

        return Result.Success();
    }

    public async Task<Result<Guid>> Delete(
        Volunteer volunteer,
        CancellationToken cancellToken = default)
    {
        _context.Volunteers.Remove(volunteer);

        await _context.SaveChangesAsync(cancellToken);

        return Result<Guid>.Success(volunteer.Id);
    }

    public async Task<Result> UpdateSocialNetworks(
        Guid volunteerId,
        ValueObjectList<SocialNetwork> socialNetworks,
        CancellationToken cancellation = default)
    {
        var socialNetworksValue = await _context.Volunteers
             .Where(v => v.Id == volunteerId)
             .Select(v => v.SocialNetworksList)
             .FirstOrDefaultAsync(cancellation);

        if (socialNetworksValue == null)
            return Result.Failure(Error.CreateErrorNotFound("Volunteer"));

        if(socialNetworksValue == socialNetworks)
            return Result.Success();

        await _context.SaveChangesAsync(cancellation);

        return Result.Success();
    }

}