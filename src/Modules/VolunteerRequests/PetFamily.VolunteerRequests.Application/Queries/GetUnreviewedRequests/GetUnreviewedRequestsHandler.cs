using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.SharedApplication.PaginationUtils.PagedResult;
using PetFamily.SharedKernel.Results;
using PetFamily.VolunteerRequests.Application.Dtos;
using PetFamily.VolunteerRequests.Application.IRepositories;

namespace PetFamily.VolunteerRequests.Application.Queries.GetUnreviewedRequests;

public class GetUnreviewedRequestsHandler(IVolunteerRequestReadRepository volunteerRequestReadRepo)
    : IQueryHandler<PagedResult<VolunteerRequestDto>, GetUnreviewedRequestsQuery>
{
    public async Task<Result<PagedResult<VolunteerRequestDto>>> Handle(
        GetUnreviewedRequestsQuery query,
        CancellationToken ct)
    {
        //TODO validate query
        var pagedResult = await volunteerRequestReadRepo.GetUnreviewedRequests(
            query.Page,
            query.PageSize,
            ct);

        return pagedResult;
    }
}

