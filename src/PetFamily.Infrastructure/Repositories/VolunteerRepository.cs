using Microsoft.EntityFrameworkCore;
using PetFamily.Application.Volunteers;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.DomainResult;
using PetFamily.Domain.Shared.ValueObjects;
using PetFamily.Domain.VolunteerAggregates.Root;

namespace PetFamily.Infrastructure.Repositories
{

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
            try
            {
                await _context.Volunteers.AddAsync(volunteer, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);

                return Result<Guid>.Success(volunteer.Id);
            }
            catch (Exception ex)
            {
                return Result<Guid>.Failure(Error.CreateErrorException(ex));
            }
        }

        public async Task<Result<List<Volunteer>>> GetByEmailOrPhone(string email, Phone phone)
        {
            List<Volunteer> volunteers = await _context.Volunteers
                .Where(v => v.Email == email ||
                v.PhoneNumber.RegionCode == phone.RegionCode &&
                v.PhoneNumber.Number == phone.Number).ToListAsync();

            if (volunteers.Count == 0)
                return Result<List<Volunteer>>.Failure(Error.CreateErrorNotFound("Volunteers"));

            return Result<List<Volunteer>>.Success(volunteers);
        }
    }
}