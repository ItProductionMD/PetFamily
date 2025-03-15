using Microsoft.Extensions.Logging;
using PetFamily.Application.IRepositories;
using PetFamily.Domain.Results;

namespace PetFamily.Application.Commands.VolunteerManagment.GetVolunteers;

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
    public async Task<Result<VolunteersResponse>> Handle(
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
