
using Microsoft.Extensions.Logging;
using PetFamily.Application.Dtos;
using PetFamily.Application.IRepositories;
using PetFamily.Domain.Results;

namespace PetFamily.Application.Queries;

public class GetVolunteersQueryHandler(
    IVolunteerReadRepository volunteerReadRepository,
    ILogger<GetVolunteersQueryHandler> logger)
{
    private readonly IVolunteerReadRepository _volunteerReadRepository = volunteerReadRepository;
    private readonly ILogger _logger= logger;

    public async Task<Result<GetVolunteersResponse>> Handle(
        GetVolunteersQuery query,
        CancellationToken cancellToken)
    {
        
        var volunteers = await _volunteerReadRepository.GetVolunteers(query, cancellToken);
        return Result.Ok(volunteers);
    }
}
