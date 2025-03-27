
using Microsoft.Extensions.Logging;
using PetFamily.Application.Abstractions;
using PetFamily.Application.Dtos;
using PetFamily.Application.IRepositories;
using PetFamily.Application.Queries.Volunteer.GetVolunteers;
using PetFamily.Domain.Results;

namespace PetFamily.Application.Queries.Volunteer.GetVolunteer;

public class GetVolunteerQueryHandler(
    IVolunteerReadRepository volunteerReadRepository,
    ILogger<GetVolunteersQueryHandler> logger) : IQueryHandler<VolunteerDto,GetVolunteerQuery>
{
    private readonly ILogger _logger = logger;
    private readonly IVolunteerReadRepository _volunteerReadRepository = volunteerReadRepository;

    public async Task<Result<VolunteerDto>> Handle(
        GetVolunteerQuery query,
        CancellationToken cancelToken)=>  
          await _volunteerReadRepository.GetByIdAsync(query.Id, cancelToken);
}
