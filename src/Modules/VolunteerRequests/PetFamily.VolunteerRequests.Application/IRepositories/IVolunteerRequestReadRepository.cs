using PetFamily.SharedApplication.PaginationUtils.PagedResult;
using PetFamily.SharedKernel.Results;
using PetFamily.VolunteerRequests.Application.Dtos;
using PetFamily.VolunteerRequests.Application.Queries.GetRequestsOnReview;

namespace PetFamily.VolunteerRequests.Application.IRepositories;

public interface IVolunteerRequestReadRepository
{
    Task<Result<VolunteerRequestDto>> GetByUserIdAsync(Guid userId, CancellationToken ct);

    Task<bool> CheckIfRequestExistAsync(Guid userId, CancellationToken ct);

    Task<Result<PagedResult<VolunteerRequestDto>>> GetUnreviewedRequests(
        int page,
        int pageSize,
        CancellationToken ct);

    Task<Result<PagedResult<VolunteerRequestDto>>> GetRequestsOnReview(
        Guid adminId,
        int page,
        int pageSize,
        VolunteerRequestsFilter filter,
        CancellationToken ct);
}
