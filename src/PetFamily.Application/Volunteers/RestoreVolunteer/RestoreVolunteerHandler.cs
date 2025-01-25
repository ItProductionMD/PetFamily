using Microsoft.Extensions.Logging;
using PetFamily.Domain.Shared.DomainResult;

namespace PetFamily.Application.Volunteers.RestoreVolunteer;

public class RestoreVolunteerHandler(
ILogger<RestoreVolunteerHandler> logger,
IVolunteerRepository volunteerRepository)
{
    private readonly IVolunteerRepository _volunteerRepository = volunteerRepository;
    private readonly ILogger<RestoreVolunteerHandler> _logger = logger;
    public async Task<Result<Guid>> Handle(Guid volunteerId, CancellationToken cancellationToken)
    {
        //--------------------------------------Get Volunteer-------------------------------------//
        var getVolunteer = await _volunteerRepository.GetById(volunteerId, cancellationToken);
        if (getVolunteer.IsFailure)
        {
            _logger.LogError("Volunteer with id {volunteerId} not found", volunteerId);
            return Result<Guid>.Failure(getVolunteer.Errors!);
        }
        var volunteer = getVolunteer.Data;
        //--------------------------------------Restore Volunteer---------------------------------//
        volunteer.Restore();
        //--------------------------------------Save Volunteer------------------------------------//
        await _volunteerRepository.Save(volunteer, cancellationToken);

        return Result<Guid>.Success(volunteer.Id);
    }
}
