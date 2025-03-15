using Microsoft.Extensions.Logging;
using PetFamily.Application.IRepositories;
using PetFamily.Domain.PetManagment.Root;
using PetFamily.Domain.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetFamily.Application.Commands.VolunteerManagment.GetVolunteer;

public class GetVolunteerHandler(
    ILogger<GetVolunteerHandler> logger,
    IVolunteerRepository volunteerRepository)
{
    private ILogger<GetVolunteerHandler> _logger = logger;
    private IVolunteerRepository _volunteerRepository = volunteerRepository; 
    public async Task<Result<Volunteer>> Handle(Guid volunteerId, CancellationToken cancelToken)
    {
        var volunteer = await _volunteerRepository.GetByIdAsync(volunteerId, cancelToken);

        return Result.Ok(volunteer);
    }
}