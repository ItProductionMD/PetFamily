using Microsoft.Extensions.Logging;
using PetFamily.Domain.Results;

namespace PetFamily.Application.Volunteers.DeleteVolunteer;

public class DeleteVolunteerHandler(
    ILogger<DeleteVolunteerHandler> logger,
    IVolunteerRepository volunteerRepository)
{
    private readonly IVolunteerRepository _volunteerRepository = volunteerRepository;
    private readonly ILogger<DeleteVolunteerHandler> _logger = logger;
    public async Task<Result<Guid>> Handle(Guid volunteerId, CancellationToken cancellationToken)
    {
        var getVolunteer = await _volunteerRepository.GetByIdAsync(volunteerId, cancellationToken);
        if (getVolunteer.IsFailure)
        {
            _logger.LogError("Fail get volunteer with id {Id} for deleting volunteer!Errors:{Errors}",
                volunteerId,getVolunteer.ConcateErrorMessages());

            return Result.Fail(getVolunteer.Errors!);
        }
        var volunteer = getVolunteer.Data!;

        await _volunteerRepository.Delete(volunteer, cancellationToken);

        _logger.LogInformation("Harddelete volunteer with id:{Id} successful!",volunteerId);

        return Result.Ok(volunteer.Id);
    }
}
