using Microsoft.Extensions.Logging;
using PetFamily.Domain.Results;

namespace PetFamily.Application.Volunteers.DeleteVolunteer;

public class SoftDeleteVolunteerHandler(
    ILogger<SoftDeleteVolunteerHandler> logger,
    IVolunteerRepository volunteerRepository)
{
    private readonly IVolunteerRepository _volunteerRepository = volunteerRepository;
    private readonly ILogger<SoftDeleteVolunteerHandler> _logger = logger;
    public async Task<Result<Guid>> Handle(Guid volunteerId, CancellationToken cancellationToken)
    {
        //--------------------------------------Get Volunteer-------------------------------------//
        var getVolunteer = await _volunteerRepository.GetByIdAsync(volunteerId, cancellationToken);
        if (getVolunteer.IsFailure)
        {
            _logger.LogError("Fail get volunteer with id {volunteerId} for delete volunteer!Errors:{Errors}",
                volunteerId, getVolunteer.ConcateErrorMessages());

            return Result.Fail(getVolunteer.Errors!);
        }
        var volunteer = getVolunteer.Data!;

        volunteer.Delete();

        await _volunteerRepository.Save(volunteer, cancellationToken);

        _logger.LogInformation("Softdelete volunteer with id:{volunteerId} successful!", volunteerId);

        return Result.Ok(volunteer.Id);
    }
}
