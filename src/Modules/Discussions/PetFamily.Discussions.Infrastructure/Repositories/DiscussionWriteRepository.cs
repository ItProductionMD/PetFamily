using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PetFamily.Discussions.Application.IRepositories;
using PetFamily.Discussions.Domain.Entities;
using PetFamily.Discussions.Infrastructure.Contexts;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;

namespace PetFamily.Discussions.Infrastructure.Repositories;

public class DiscussionWriteRepository(
    DiscussionWriteDbContext context,
    ILogger<DiscussionWriteRepository> logger) : IDiscussionWriteRepository
{
    public async Task AddAndSaveAsync(Discussion discussion, CancellationToken ct)
    {
        await context.Discussions.AddAsync(discussion, ct);
        await context.SaveChangesAsync(ct);
    }

    public async Task<Result<Discussion>> GetById(Guid discussionId, CancellationToken ct)
    {
        var discussion = await context.Discussions
            .Include(d => d.Messages)
            .FirstOrDefaultAsync(d => d.Id == discussionId, ct);

        if (discussion == null)
        {
            logger.LogError("Discussion with Id:{DiscussionId} not found", discussionId);
            return Result.Fail(Error.NotFound($"Discussion with Id:{discussionId} not found"));
        }

        logger.LogInformation("Discussion with Id:{DiscussionId} found", discussionId);

        return Result.Ok(discussion);
    }

    public async Task SaveAsync(CancellationToken ct)
    {
        await context.SaveChangesAsync(ct);
    }
}
