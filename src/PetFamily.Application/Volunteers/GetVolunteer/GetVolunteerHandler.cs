using Microsoft.Extensions.Logging;
using PetFamily.Domain.Shared.DomainResult;
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
        var getVolunteer = await _volunteerRepository.GetById(volunteerId, cancelToken);
        if (getVolunteer.IsFailure)
        {
            _logger.LogError("Volunteer with id {volunteerId} not found", volunteerId);
            return Result<Volunteer>.Failure(getVolunteer.Errors!);
        }
        return Result<Volunteer>.Success(getVolunteer.Data);
    }
}