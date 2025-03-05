using Microsoft.Extensions.Logging;
using PetFamily.Domain.Results;

namespace PetFamily.Application.Volunteers.RestoreVolunteer;

public class RestoreVolunteerHandler(
ILogger<RestoreVolunteerHandler> logger,
IVolunteerRepository volunteerRepository)
{
    private readonly IVolunteerRepository _volunteerRepository = volunteerRepository;
    private readonly ILogger<RestoreVolunteerHandler> _logger = logger;
    public async Task<UnitResult> Handle(Guid volunteerId, CancellationToken cancelToken)
    {
        var volunteer = await _volunteerRepository.GetByIdAsync(volunteerId, cancelToken);
        
        volunteer.Restore();

        await _volunteerRepository.Save(volunteer, cancelToken);

        _logger.LogInformation("Restore volunteer with Id:{Id} successful",volunteerId);

        return UnitResult.Ok();
    }
}
