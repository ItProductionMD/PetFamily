using Microsoft.Extensions.Logging;
using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.SharedApplication.IUserContext;
using PetFamily.SharedKernel.Results;
using PetFamily.VolunteerRequests.Application.Dtos;
using PetFamily.VolunteerRequests.Application.IRepositories;
using System.Security.Cryptography;

namespace PetFamily.VolunteerRequests.Application.Queries.GetRequest;

public class GetOwnVolunteerRequestHandler(
    ILogger<GetOwnVolunteerRequestHandler> logger,
    IVolunteerRequestReadRepository requestReadRepo) : IQueryHandler<VolunteerRequestDto? ,GetOwnVolunteerRequestQuery>
{
    private readonly ILogger _logger = logger;
    private readonly IVolunteerRequestReadRepository _requestReadRepo = requestReadRepo;
    public async Task<Result<VolunteerRequestDto>> Handle(GetOwnVolunteerRequestQuery query, CancellationToken ct)
    {
        var userId = query.UserId;

        return (await _requestReadRepo.GetByUserIdAsync(userId, ct));
    }
}
