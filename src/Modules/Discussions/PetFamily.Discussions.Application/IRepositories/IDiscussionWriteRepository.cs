using PetFamily.Discussions.Domain.Entities;
using PetFamily.SharedKernel.Results;

namespace PetFamily.Discussions.Application.IRepositories;

public interface IDiscussionWriteRepository
{
    Task AddAndSaveAsync(Discussion discussion, CancellationToken ct);
    Task<Result<Discussion>> GetById(Guid discussionId, CancellationToken ct);
    Task SaveAsync(CancellationToken ct);
}
