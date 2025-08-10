using PetFamily.Discussions.Application.Dtos;
using PetFamily.SharedApplication.PaginationUtils;
using PetFamily.SharedKernel.Results;

namespace PetFamily.Discussions.Application.IRepositories;

public interface IDiscussionReadRepository
{
    Task<Result<DiscussionDto>> GetById(
        Guid DiscussionId, 
        PaginationParams paginationParams, 
        CancellationToken ct);
}
