using Dapper;
using Microsoft.Extensions.Logging;
using PetFamily.Discussions.Application.Dtos;
using PetFamily.Discussions.Application.IRepositories;
using PetFamily.SharedApplication.Abstractions;
using PetFamily.SharedApplication.PaginationUtils;
using PetFamily.SharedInfrastructure.Dapper.Extensions;
using PetFamily.SharedInfrastructure.Dapper.ScaffoldedClasses;
using PetFamily.SharedInfrastructure.Shared.Dapper.ScaffoldedClassesPreview;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using static PetFamily.SharedInfrastructure.Dapper.Extensions.DapperLoggerExtensions;
using DiscussionsTable = PetFamily.SharedInfrastructure.Dapper.ScaffoldedClasses.DiscussionsTable;

namespace PetFamily.Discussions.Infrastructure.Repositories;

public class DiscussionReadRepository(
    IDbConnectionFactory dbConnectionFactory,
    ILogger<DiscussionReadRepository> logger) : IDiscussionReadRepository
{
    public async Task<Result<DiscussionDto>> GetById(
        Guid discussionId,
        PaginationParams paginationParams,
        CancellationToken ct = default)
    {
        const string sqlDiscussion = $@"
        SELECT
            d.{DiscussionsTable.Id} AS Id,
            d.{DiscussionsTable.IsClosed} AS IsClosed,
            d.{DiscussionsTable.ParticipantIds} AS ParticipantIds
        FROM {DiscussionsTable.TableFullName} d
        WHERE d.{DiscussionsTable.Id} = @DiscussionId;";

        var discussionCmd = new CommandDefinition(
            commandText: sqlDiscussion,
            parameters: new { DiscussionId = discussionId },
            cancellationToken: ct
        );

        await using var connection = await dbConnectionFactory.CreateOpenConnectionAsync();

        logger.DapperLogSqlQuery(discussionCmd.CommandText, discussionCmd.Parameters);

        var discussion = await connection.QuerySingleOrDefaultAsync<DiscussionDto>(discussionCmd);
        if (discussion == null)
        {
            logger.LogWarning("Discussion with id:{DiscussionId} not found", discussionId);
            return Result.Fail(Error.NotFound($"Discussion with id:{discussionId}"));
        }

        const string sqlMessagesBase = $@"
        SELECT
            m.{MessagesTable.Id} AS Id,
            m.{MessagesTable.AuthorId} AS AuthorId,
            m.{MessagesTable.Text} AS Text,
            m.{MessagesTable.CreatedAt} AS CreatedAt
        FROM {MessagesTable.TableFullName} m
        WHERE m.{MessagesTable.DiscussionId} = @DiscussionId
        ORDER BY m.{MessagesTable.CreatedAt}";

        var pagedQueryCmd = new CommandDefinition(
            commandText: sqlMessagesBase,
            parameters: new
            {
                DiscussionId = discussionId,
                PageSize = paginationParams.PageSize,
                Offset = paginationParams.Offset
            },
            cancellationToken: ct
        );

        var (totalCount, messages) = await connection
            .GetPagedQuery<DiscussionMessageDto, DiscussionReadRepository>(pagedQueryCmd, logger);

        discussion.Messages = messages.ToList();

        logger.LogInformation("Discussion with id:{DiscussionId} found with {MessageCount} messages " +
            "(Total messages: {TotalCount})",
            discussionId, messages.Count, totalCount);

        return Result.Ok(discussion);
    }
}

