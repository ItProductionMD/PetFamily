using Dapper;
using Microsoft.Extensions.Logging;
using PetFamily.Application.Abstractions;
using PetFamily.SharedApplication.PagedResult;
using PetFamily.SharedInfrastructure.Dapper.ScaffoldedClasses;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using PetFamily.VolunteerRequests.Application.Dtos;
using PetFamily.VolunteerRequests.Application.IRepositories;
using PetFamily.VolunteerRequests.Application.Queries.GetRequestsOnReview;
using PetFamily.VolunteerRequests.Domain.Entities;
using PetFamily.VolunteerRequests.Domain.Enums;
using System.Net.Http.Headers;

namespace PetFamily.VolunteerRequests.Infrastructure.Repositories.Read;

public class VolunteerRequestReadRepository(
    IDbConnectionFactory dbConnectionFactory,
    ILogger<VolunteerRequestReadRepository> logger) : IVolunteerRequestReadRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory = dbConnectionFactory;
    private readonly ILogger<VolunteerRequestReadRepository> _logger = logger;

    public async Task<bool> CheckIfRequestExistAsync(Guid userId, CancellationToken ct)
    {
        await using var connection = await _dbConnectionFactory.CreateOpenConnectionAsync();

        const string sql = $@"
            SELECT COUNT(1) FROM {VolunteerRequestsTable.TableFullName}
            WHERE {VolunteerRequestsTable.UserId} = @UserId;
        ";

        var count = await connection.ExecuteScalarAsync<int>(sql, new { UserId = userId });
        return count > 0;
    }

    public async Task<VolunteerRequestDto?> GetByUserIdAsync(Guid userId, CancellationToken ct)
    {
        var sql = $@"
                SELECT 
                {VolunteerRequestsTable.Id} As Id,
                {VolunteerRequestsTable.UserId} As UserId,
                {VolunteerRequestsTable.FirstName} As FirstName,
                {VolunteerRequestsTable.LastName} As LastName,
                {VolunteerRequestsTable.Description} As Description,
                {VolunteerRequestsTable.ExperienceYears} As ExperienceYears,
                {VolunteerRequestsTable.DocumentName} As DocumentName,
                {VolunteerRequestsTable.CreatedAt} As CreatedAt,
                {VolunteerRequestsTable.RequestStatus} As RequestStatus
                FROM {VolunteerRequestsTable.TableFullName}
                WHERE {VolunteerRequestsTable.UserId} = @UserId 
                LIMIT 1";

        await using var dbConnection = await _dbConnectionFactory.CreateOpenConnectionAsync();

        _logger.LogInformation("EXECUTING QUERY: {sql};", sql);

        var result = await dbConnection.QuerySingleOrDefaultAsync<VolunteerRequestDto>(
            sql,
            new
            {
                UserId = userId
            });

        return result;
    }

    public async Task<Result<PagedResult<VolunteerRequestDto>>> GetRequestsOnReview(
        Guid adminId,
        int page,
        int pageSize,
        VolunteerRequestsFilter filter,
        CancellationToken ct)
    {
        var offset = Offset.Calculate(page, pageSize);

        var statusFilter = filter.Statuses.Count > 0
            ? @$"AND {VolunteerRequestsTable.RequestStatus} IN @Statuses"
            : string.Empty;

        var sql = $@"
                SELECT 
                {VolunteerRequestsTable.Id} As Id,
                {VolunteerRequestsTable.UserId} As UserId,
                {VolunteerRequestsTable.FirstName} As FirstName,
                {VolunteerRequestsTable.LastName} As LastName,
                {VolunteerRequestsTable.Description} As Description,
                {VolunteerRequestsTable.ExperienceYears} As ExperienceYears,
                {VolunteerRequestsTable.DocumentName} As DocumentName,
                {VolunteerRequestsTable.CreatedAt} As CreatedAt,
                {VolunteerRequestsTable.RequestStatus} As RequestStatus
                FROM {VolunteerRequestsTable.TableFullName}
                WHERE {VolunteerRequestsTable.AdminId} = @AdminId
                {statusFilter}
                ORDER BY {VolunteerRequestsTable.CreatedAt}
                LIMIT @Limit OFFSET @Offset;";

        var sqlCount = $@"
                SELECT COUNT(1)
                FROM {VolunteerRequestsTable.TableFullName}
                WHERE {VolunteerRequestsTable.AdminId} = @AdminId
                {statusFilter}";

        await using var dbConnection = await _dbConnectionFactory.CreateOpenConnectionAsync();

        _logger.LogInformation("EXECUTING QUERY: {sqlCount}; with params:{statuses},{adminId}",
            sqlCount, filter.Statuses, adminId);

        var count = await dbConnection.ExecuteScalarAsync<int>(
            sqlCount,
            new
            {
                AdminId = adminId,
                Statuses = filter.Statuses
            });

        _logger.LogInformation("EXECUTING QUERY: {sql} ; with params: {pageSize}, {offset},{statuses}" +
            ",{adminId}", sql, pageSize, offset, filter.Statuses, adminId);

        var items = await dbConnection.QueryAsync<VolunteerRequestDto>(
             sql,
             new
             {
                 AdminId = adminId,
                 Statuses = filter.Statuses,
                 Offset = offset,
                 Limit = pageSize
             }
        );

        var pageResult = new PagedResult<VolunteerRequestDto>(items.ToList(), count, pageSize);

        return Result.Ok(pageResult);
    }

    public async Task<Result<PagedResult<VolunteerRequestDto>>> GetUnreviewedRequests(
        int page,
        int pageSize,
        CancellationToken ct)
    {
        var status = RequestStatus.Created.ToString();

        var offset = Offset.Calculate(page, pageSize);

        var sql = $@"
                SELECT 
                {VolunteerRequestsTable.Id} As Id,
                {VolunteerRequestsTable.UserId} As UserId,
                {VolunteerRequestsTable.FirstName} As FirstName,
                {VolunteerRequestsTable.LastName} As LastName,
                {VolunteerRequestsTable.Description} As Description,
                {VolunteerRequestsTable.ExperienceYears} As ExperienceYears,
                {VolunteerRequestsTable.DocumentName} As DocumentName,
                {VolunteerRequestsTable.CreatedAt} As CreatedAt,
                {VolunteerRequestsTable.RequestStatus} As RequestStatus
                FROM {VolunteerRequestsTable.TableFullName}
                WHERE {VolunteerRequestsTable.AdminId} IS NULL
                    AND {VolunteerRequestsTable.RequestStatus} = @Status
                ORDER BY {VolunteerRequestsTable.CreatedAt}
                LIMIT @Limit OFFSET @Offset;";

        var sqlCount = $@"
                SELECT COUNT(1)
                FROM {VolunteerRequestsTable.TableFullName}
                WHERE {VolunteerRequestsTable.AdminId} IS NULL 
                AND {VolunteerRequestsTable.RequestStatus} = @Status";

        await using var dbConnection = await _dbConnectionFactory.CreateOpenConnectionAsync();

        _logger.LogInformation("EXECUTING QUERY: {sqlCount}; with params:{status}", sqlCount, status);

        var count = await dbConnection.ExecuteScalarAsync<int>(sqlCount, new { Status = status });

        _logger.LogInformation("EXECUTING QUERY: {sql} ; with params: {pageSize}, {offset}",
            sql, pageSize, offset);

        var items = await dbConnection.QueryAsync<VolunteerRequestDto>(
             sql,
             new
             {
                 Status = status,
                 Offset = offset,
                 Limit = pageSize
             }
        );

        var pageResult = new PagedResult<VolunteerRequestDto>(items.ToList(), count, pageSize);

        return Result.Ok(pageResult);
    }
}
