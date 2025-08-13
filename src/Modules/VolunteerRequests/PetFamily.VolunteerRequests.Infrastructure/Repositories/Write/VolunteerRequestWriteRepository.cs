using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using PetFamily.VolunteerRequests.Application.IRepositories;
using PetFamily.VolunteerRequests.Domain.Entities;
using PetFamily.VolunteerRequests.Infrastructure.Contexts;

namespace PetFamily.VolunteerRequests.Infrastructure.Repositories.Write;

public class VolunteerRequestWriteRepository(
    VolunteerRequestWriteDbContext dbContext,
    ILogger<VolunteerRequestWriteRepository> logger)
    : IVolunteerRequestWriteRepository
{
    public async Task AddAsync(VolunteerRequest volunteerRequest, CancellationToken ct)
    {
        await dbContext.VolunteerRequests.AddAsync(volunteerRequest, ct);

        logger.LogInformation("Add volunteer request for userId:{id} successful!",
            volunteerRequest.UserId);
    }

    public async Task<Result<VolunteerRequest>> GetByIdAsync(Guid volunteerRequestId, CancellationToken ct)
    {
        var volunteerRequest = await dbContext.VolunteerRequests
                .FirstOrDefaultAsync(vr => vr.Id == volunteerRequestId, ct);

        if (volunteerRequest == null)
        {
            logger.LogWarning("Volunteer request with id:{id} not found", volunteerRequestId);

            return UnitResult.Fail(Error.NotFound("Volunteer request not found"));
        }
        logger.LogInformation("Volunteer request with id:{id} retrieved successfully", volunteerRequestId);

        return Result.Ok(volunteerRequest);
    }

    public async Task SaveAsync(CancellationToken ct)
    {
        var result = await dbContext.SaveChangesAsync(ct);

        logger.LogInformation("Changes saved successfully");
    }
}
