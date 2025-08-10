using PetFamily.Application.Abstractions.CQRS;
using PetFamily.SharedApplication.PagedResult;
using PetFamily.SharedKernel.Results;
using PetFamily.VolunteerRequests.Application.Dtos;
using PetFamily.VolunteerRequests.Application.IRepositories;

namespace PetFamily.VolunteerRequests.Application.Queries.GetUnreviewedRequests;

public class GetUnreviewedRequestsHandler(
    IVolunteerRequestReadRepository repository
) : IQueryHandler<PagedResult<VolunteerRequestDto>, GetUnreviewedRequestsQuery>
{
    public async Task<Result<PagedResult<VolunteerRequestDto>>> Handle(
        GetUnreviewedRequestsQuery query,
        CancellationToken ct)
    {
        //TODO validate query
        var pagedResult = await repository.GetUnreviewedRequests(query.Page, query.PageSize, ct);

        return pagedResult;
    }
}

