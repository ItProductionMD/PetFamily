using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using PetFamily.VolunteerRequests.Application.IRepositories;
using PetFamily.VolunteerRequests.Domain.Entities;
using PetFamily.VolunteerRequests.Infrastructure.Contexts;

namespace PetFamily.VolunteerRequests.Infrastructure.Repositories.Write;

public class VolunteerRequestWriteRepository(
    ILogger<VolunteerRequestWriteRepository> logger,
    VolunteerRequestDbContext volunteerRequestDbContext) : IVolunteerRequestWriteRepository
{
    private readonly ILogger<VolunteerRequestWriteRepository> _logger = logger;
    private readonly VolunteerRequestDbContext _dbContext = volunteerRequestDbContext;

    public async Task AddAsync(VolunteerRequest volunteerRequest, CancellationToken ct)
    {
        await _dbContext.VolunteerRequests.AddAsync(volunteerRequest, ct);

        _logger.LogInformation("Add volunteer request for userId:{id} successful!",
            volunteerRequest.UserId);
    }

    public async Task<Result<VolunteerRequest>> GetByIdAsync(Guid volunteerRequestId, CancellationToken ct)
    {
        var volunteerRequest = await _dbContext.VolunteerRequests
                .FirstOrDefaultAsync(vr => vr.Id == volunteerRequestId, ct);

        if (volunteerRequest == null)
        {
            _logger.LogWarning("Volunteer request with id:{id} not found", volunteerRequestId);

            return UnitResult.Fail(Error.NotFound("Volunteer request not found"));
        }
        _logger.LogInformation("Volunteer request with id:{id} retrieved successfully", volunteerRequestId);

        return Result.Ok(volunteerRequest);
    }

    public async Task SaveAsync(CancellationToken ct)
    {
        var result = await _dbContext.SaveChangesAsync(ct);

        _logger.LogInformation("Changes saved successfully");
    }
}
