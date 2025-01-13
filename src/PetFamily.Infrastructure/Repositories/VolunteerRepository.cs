using PetFamily.Application.Volunteers;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.DomainResult;
using PetFamily.Domain.VolunteerAggregates.Root;

namespace PetFamily.Infrastructure.Repositories;

public class VolunteerRepository:IVolunteerRepository
{
    private readonly AppDbContext _context;
    public VolunteerRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task<Result<Guid>> Add(Volunteer volunteer,CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.Volunteers.AddAsync(volunteer, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return Result<Guid>.Success(volunteer.Id);
        }
        catch (Exception ex) 
        {
            Console.WriteLine(ex);
           return Result<Guid>.Failure(Error.CreateErrorException());
        }
    }
}
