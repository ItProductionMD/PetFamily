using Microsoft.Extensions.Logging;
using PetFamily.Domain.Results;

namespace PetFamily.Application.Volunteers.DeleteVolunteer;

public class DeleteVolunteerHandler(
    ILogger<DeleteVolunteerHandler> logger,
    IVolunteerRepository volunteerRepository)
{
    private readonly IVolunteerRepository _volunteerRepository = volunteerRepository;
    private readonly ILogger<DeleteVolunteerHandler> _logger = logger;
    public async Task<Result<Guid>> Handle(Guid volunteerId, CancellationToken cancelToken)
    {
        var volunteer = await _volunteerRepository.GetByIdAsync(volunteerId, cancelToken);
        
        await _volunteerRepository.Delete(volunteer, cancelToken);

        _logger.LogInformation("Hard delete volunteer with id:{Id} successful!",volunteerId);

        return Result.Ok(volunteer.Id);
    }
}
