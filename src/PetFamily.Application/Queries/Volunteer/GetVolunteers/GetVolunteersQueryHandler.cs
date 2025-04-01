
using Microsoft.Extensions.Logging;
using PetFamily.Application.Abstractions;
using PetFamily.Application.Dtos;
using PetFamily.Application.IRepositories;
using PetFamily.Domain.Results;

namespace PetFamily.Application.Queries.Volunteer.GetVolunteers;

public class GetVolunteersQueryHandler(
    IVolunteerReadRepository volunteerReadRepository,
    ILogger<GetVolunteersQueryHandler> logger) : IQueryHandler<GetVolunteersResponse, GetVolunteersQuery>
{
    private readonly IVolunteerReadRepository _volunteerReadRepository = volunteerReadRepository;
    private readonly ILogger _logger = logger;

    public async Task<Result<GetVolunteersResponse>> Handle(
        GetVolunteersQuery query,
        CancellationToken cancellToken)
    {
        //validate query
        var getVolunteersResponse = await _volunteerReadRepository.GetVolunteers(query, cancellToken);
        return Result.Ok(getVolunteersResponse);
    }
}
