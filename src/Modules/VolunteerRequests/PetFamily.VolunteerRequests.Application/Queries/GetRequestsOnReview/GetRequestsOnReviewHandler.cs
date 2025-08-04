using Microsoft.Extensions.Logging;
using PetFamily.Application.Abstractions.CQRS;
using PetFamily.SharedApplication.IUserContext;
using PetFamily.SharedApplication.PagedResult;
using PetFamily.SharedKernel.Results;
using PetFamily.VolunteerRequests.Application.Dtos;
using PetFamily.VolunteerRequests.Application.IRepositories;

namespace PetFamily.VolunteerRequests.Application.Queries.GetRequestsOnReview;

public class GetRequestsOnReviewHandler(
    ILogger<GetRequestsOnReviewHandler> logger,
    IVolunteerRequestReadRepository requestReadRepository,
    IUserContext userContext) : IQueryHandler<PagedResult<VolunteerRequestDto>, GetRequestsOnReviewQuery>
{
    private readonly ILogger<GetRequestsOnReviewHandler> _logger = logger;
    private readonly IUserContext _userContext = userContext;
    private readonly IVolunteerRequestReadRepository _requestReadRepository = requestReadRepository;
    public async Task<Result<PagedResult<VolunteerRequestDto>>> Handle(
        GetRequestsOnReviewQuery query,
        CancellationToken ct)
    {
        GetRequestsOnReviewValidator.Validate(query);

        var adminId = _userContext.GetUserId();

        var pageResult = await _requestReadRepository.GetRequestsOnReview(
            adminId,
            query.Page,
            query.PageSize,
            query.Filter,
            ct);

        return pageResult;
    }
}
