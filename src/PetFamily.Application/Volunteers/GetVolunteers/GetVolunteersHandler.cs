using Microsoft.Extensions.Logging;
using PetFamily.Application.Volunteers.Dtos;
using PetFamily.Domain.Results;

namespace PetFamily.Application.Volunteers.GetVolunteers;

public class GetVolunteersHandler
{
    private readonly IVolunteerRepository _volunteerRepository;
    private readonly ILogger<GetVolunteersHandler> _logger;
    public GetVolunteersHandler(
        IVolunteerRepository volunteerRepository,
        ILogger<GetVolunteersHandler> logger)
    {
        _volunteerRepository = volunteerRepository;
        _logger = logger;
    }
    public async Task<Result<GetVolunteersResponse>> Handle(
        GetVolunteersCommand command,
        CancellationToken cancelToken)
    {
        //TODO validateCommand
        var volunteers = await _volunteerRepository.GetVolunteersAsync(
            command.PageNumber,
            command.maxItemsOnPage, 
            cancelToken);

        return Result.Ok(volunteers);
    }
}
