using Microsoft.Extensions.Logging;
using PetFamily.Domain.Results;
using PetFamily.Domain.VolunteerAggregates.Root;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetFamily.Application.Volunteers.GetVolunteer;

public class GetVolunteerHandler(
    ILogger<GetVolunteerHandler> logger,
    IVolunteerRepository volunteerRepository)
{
    private ILogger<GetVolunteerHandler> _logger = logger;
    private IVolunteerRepository _volunteerRepository = volunteerRepository; 
    public async Task<Result<Volunteer>> Handle(Guid volunteerId, CancellationToken cancelToken)
    {
        var getVolunteer = await _volunteerRepository.GetByIdAsync(volunteerId, cancelToken);
        if (getVolunteer.IsFailure)
        {
            _logger.LogError("Fail to get volunteer with id {volunteerId}!Errors:{Errors}",
                volunteerId, getVolunteer.ConcateErrorMessages());

            return Result.Fail(getVolunteer.Errors!);
        }
        _logger.LogInformation("Get volunteer with {volunteerId} succesfully!",volunteerId);

        return Result.Ok(getVolunteer.Data);
    }
}