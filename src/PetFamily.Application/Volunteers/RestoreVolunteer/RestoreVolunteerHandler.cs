using Microsoft.Extensions.Logging;
using PetFamily.Domain.Results;

namespace PetFamily.Application.Volunteers.RestoreVolunteer;

public class RestoreVolunteerHandler(
ILogger<RestoreVolunteerHandler> logger,
IVolunteerRepository volunteerRepository)
{
    private readonly IVolunteerRepository _volunteerRepository = volunteerRepository;
    private readonly ILogger<RestoreVolunteerHandler> _logger = logger;
    public async Task<UnitResult> Handle(Guid volunteerId, CancellationToken cancellationToken)
    {
        var getVolunteer = await _volunteerRepository.GetByIdAsync(volunteerId, cancellationToken);
        if (getVolunteer.IsFailure)
        {
            _logger.LogError("Fail get volunteer with id {volunteerId} fore restore volunteer!", volunteerId);

            return UnitResult.Fail(getVolunteer.Errors);
        }
        var volunteer = getVolunteer.Data!;

        volunteer.Restore();

        await _volunteerRepository.Save(volunteer, cancellationToken);

        _logger.LogInformation("Restore volunteer with Id:{Id} successful",volunteerId);

        return UnitResult.Ok();
    }
}
