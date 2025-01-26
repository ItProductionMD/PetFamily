using Microsoft.Extensions.Logging;
using PetFamily.Domain.Shared.DomainResult;

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
        var getVolunteer = await _volunteerRepository.GetById(volunteerId, cancellationToken);
        if (getVolunteer.IsFailure)
        {
            _logger.LogError("Volunteer with id {volunteerId} not found", volunteerId);
            return Result<Guid>.Failure(getVolunteer.Errors!);
        }
        var volunteer = getVolunteer.Data;
        //----------------------------------Soft Delete Volunteer---------------------------------//
        volunteer.Delete();
        //--------------------------------------Save Volunteer------------------------------------//
        await _volunteerRepository.Save(volunteer, cancellationToken);

        return Result<Guid>.Success(volunteer.Id);
    }
}
