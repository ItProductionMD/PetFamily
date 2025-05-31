using Microsoft.Extensions.Logging;
using PetFamily.Application.Abstractions.CQRS;
using PetFamily.SharedKernel.Results;
using Volunteers.Application.IRepositories;

namespace Volunteers.Application.Queries.GetVolunteers;

public class GetVolunteersQueryHandler(
    IVolunteerReadRepository repository,
    ILogger<GetVolunteersQueryHandler> logger)
    : IQueryHandler<GetVolunteersResponse, GetVolunteersQuery>
{
    private readonly IVolunteerReadRepository _repository = repository;
    private readonly ILogger _logger = logger;

    public async Task<Result<GetVolunteersResponse>> Handle(
        GetVolunteersQuery query,
        CancellationToken ct)
    {
        //validate query
        var getVolunteersResponse = await _repository.GetVolunteers(query, ct);
        return Result.Ok(getVolunteersResponse);
    }
}
