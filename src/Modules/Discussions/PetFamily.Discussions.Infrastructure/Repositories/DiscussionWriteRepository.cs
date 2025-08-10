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
    private readonly DiscussionWriteDbContext _context = context;
    private readonly ILogger<DiscussionWriteRepository> _logger = logger;
    public async Task AddAndSaveAsync(Discussion discussion, CancellationToken ct)
    {
        await _context.Discussions.AddAsync(discussion, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<Result<Discussion>> GetById(Guid discussionId, CancellationToken ct)
    {
        var discussion = await _context.Discussions
            .Include(d => d.Messages)
            .FirstOrDefaultAsync(d => d.Id == discussionId, ct);

        if(discussion == null)
        {
            _logger.LogError("Discussion with Id:{DiscussionId} not found", discussionId);
            return Result.Fail(Error.NotFound($"Discussion with Id:{discussionId} not found"));
        }

        _logger.LogInformation("Discussion with Id:{DiscussionId} found", discussionId);

        return Result.Ok(discussion);
    }

    public async Task SaveAsync(CancellationToken ct)
    {
        await _context.SaveChangesAsync(ct);
    }
}
