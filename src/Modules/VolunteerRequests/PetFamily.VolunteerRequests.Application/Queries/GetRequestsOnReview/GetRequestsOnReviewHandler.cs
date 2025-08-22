using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.SharedApplication.PaginationUtils.PagedResult;
using PetFamily.SharedKernel.Results;
using PetFamily.VolunteerRequests.Application.Dtos;
using PetFamily.VolunteerRequests.Application.IRepositories;

namespace PetFamily.VolunteerRequests.Application.Queries.GetRequestsOnReview;

public class GetRequestsOnReviewHandler(IVolunteerRequestReadRepository requestReadRepository)
    : IQueryHandler<PagedResult<VolunteerRequestDto>, GetRequestsOnReviewQuery>
{
    public async Task<Result<PagedResult<VolunteerRequestDto>>> Handle(
        GetRequestsOnReviewQuery query,
        CancellationToken ct)
    {
        query.Validate();

        var pageResult = await requestReadRepository.GetRequestsOnReview(
            query.AdminId,
            query.Page,
            query.PageSize,
            query.Filter,
            ct);

        return pageResult;
    }
}
