using Microsoft.Extensions.Logging;
using PetFamily.Application.Abstractions.CQRS;
using PetFamily.SharedApplication.IUserContext;
using PetFamily.SharedKernel.Results;
using PetFamily.VolunteerRequests.Application.Dtos;
using PetFamily.VolunteerRequests.Application.IRepositories;

namespace PetFamily.VolunteerRequests.Application.Queries.GetRequest;

public class GetOwnVolunteerRequestHandler(
    IUserContext userContext,
    ILogger<GetOwnVolunteerRequestHandler> logger,
    IVolunteerRequestReadRepository requestReadRepository) : IQueryHandler<VolunteerRequestDto? ,GetOwnVolunteerRequestQuery>
{
    private readonly ILogger _logger = logger;
    private readonly IUserContext _userContext = userContext;
    private readonly IVolunteerRequestReadRepository _requestReadRepository = requestReadRepository;
    public async Task<Result<VolunteerRequestDto?>> Handle(GetOwnVolunteerRequestQuery query, CancellationToken ct)
    {
        var userId = _userContext.GetUserId();

        var request = await _requestReadRepository.GetByUserIdAsync(userId, ct);

        return Result.Ok(request);
    }
}
