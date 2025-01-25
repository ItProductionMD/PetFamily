using Microsoft.Extensions.Logging;
using PetFamily.Domain.Shared.DomainResult;
using PetFamily.Domain.VolunteerAggregates.Root;

namespace PetFamily.Application.Volunteers.DeleteVolunteer;

public class DeleteVolunteerHandler(
    ILogger<DeleteVolunteerHandler> logger,
    IVolunteerRepository volunteerRepository)
{
    private readonly IVolunteerRepository _volunteerRepository = volunteerRepository;
    private readonly ILogger<DeleteVolunteerHandler> _logger = logger;
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
        //--------------------------------------Delete Volunteer----------------------------------//
        await _volunteerRepository.Delete(volunteer, cancellationToken);

        return Result<Guid>.Success(volunteer.Id);
    }
}
