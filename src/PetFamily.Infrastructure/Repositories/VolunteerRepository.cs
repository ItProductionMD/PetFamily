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

        public async Task<Result> CheckVolunteerContactAvailability(string email, Phone phone)
        {
            var volunteer = await _context.Volunteers.FirstOrDefaultAsync(v => v.Email == email ||

                v.PhoneNumber.RegionCode == phone.RegionCode && v.PhoneNumber.Number == phone.Number);

            if (volunteer != null)
            {
                List<Error> errors = [];

                if (volunteer.PhoneNumber == phone)
                {
                    var error = Error.CreateCustomError(
                        code: "value.is.invalid",
                        message: "Phone is already exists",
                        errorType: ErrorType.Validation,
                        valueName: "Phone");

                    errors.Add(error);
                }
                if (volunteer.Email == email)
                {
                    var error = Error.CreateCustomError(
                        code: "value.is.invalid",
                        message: "Email is already exists",
                        errorType: ErrorType.Validation,
                        valueName: "Email");

                    errors.Add(error);
                }
                return Result.Failure(errors!);
            }
            return Result.Success();

        }
    }
}