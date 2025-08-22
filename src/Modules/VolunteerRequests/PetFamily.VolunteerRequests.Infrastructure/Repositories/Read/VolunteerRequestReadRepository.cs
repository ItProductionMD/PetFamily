using Dapper;
using Microsoft.Extensions.Logging;
using PetFamily.SharedApplication.Abstractions;
using PetFamily.SharedApplication.PaginationUtils.PagedResult;
using PetFamily.SharedInfrastructure.Dapper.Extensions;
using PetFamily.SharedInfrastructure.Dapper.ScaffoldedClasses;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using PetFamily.VolunteerRequests.Application.Dtos;
using PetFamily.VolunteerRequests.Application.IRepositories;
using PetFamily.VolunteerRequests.Application.Queries.GetRequestsOnReview;
using PetFamily.VolunteerRequests.Domain.Enums;

namespace PetFamily.VolunteerRequests.Infrastructure.Repositories.Read;

public class VolunteerRequestReadRepository(
    IDbConnectionFactory dbConnectionFactory,
    ILogger<VolunteerRequestReadRepository> logger) : IVolunteerRequestReadRepository
{
    public async Task<bool> CheckIfRequestExistAsync(Guid userId, CancellationToken ct)
    {
        await using var connection = await dbConnectionFactory.CreateOpenConnectionAsync();

        const string sql = $@"
            SELECT COUNT(1) FROM {VolunteerRequestsTable.TableFullName}
            WHERE {VolunteerRequestsTable.UserId} = @UserId;
        ";

        var count = await connection.ExecuteScalarAsync<int>(sql, new { UserId = userId });
        return count > 0;
    }

    public async Task<Result<VolunteerRequestDto>> GetByUserIdAsync(Guid userId, CancellationToken ct)
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

        await using var dbConnection = await dbConnectionFactory.CreateOpenConnectionAsync();

        var sqlParameters = new { UserId = userId };

        logger.DapperLogSqlQuery(sql, sqlParameters);

        var volunteerRequestDto = await dbConnection.QuerySingleOrDefaultAsync<VolunteerRequestDto>(
            sql,
            sqlParameters);

        if (volunteerRequestDto == null)
        {
            logger.LogWarning("Volunteer request with userId: {UserId} not found", userId);
            return Result.Fail(Error.NotFound($"VolunteerRequest request with userI: {userId}"));
        }

        logger.LogInformation("Volunteer request with userId: {UserId} found successful!", userId);

        return Result.Ok(volunteerRequestDto);
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

        var sqlVolunteerRequest = $@"
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

        var sqlCountParameters = new
        {
            AdminId = adminId,
            Statuses = filter.Statuses
        };

        var sqlVolunteerRequestParameters = new
        {
            AdminId = adminId,
            Statuses = filter.Statuses,
            Offset = offset,
            Limit = pageSize
        };

        await using var dbConnection = await dbConnectionFactory.CreateOpenConnectionAsync();

        logger.DapperLogSqlQuery(sqlCount, sqlCountParameters);

        var count = await dbConnection.ExecuteScalarAsync<int>(
            sqlCount,
            sqlCountParameters);

        logger.DapperLogSqlQuery(sqlVolunteerRequest, sqlVolunteerRequestParameters);

        var items = await dbConnection.QueryAsync<VolunteerRequestDto>(
             sqlVolunteerRequest,
             sqlVolunteerRequestParameters
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

        var sqlVolunteerRequest = $@"
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

        var sqlCountParameters = new { Count = sqlCount };

        await using var dbConnection = await dbConnectionFactory.CreateOpenConnectionAsync();

        logger.DapperLogSqlQuery(sqlCount, sqlCountParameters);

        var count = await dbConnection.ExecuteScalarAsync<int>(sqlCount, new { Status = status });

        var sqlVolunteerRequestParameters = new
        {
            Status = status,
            Offset = offset,
            Limit = pageSize
        };

        logger.DapperLogSqlQuery(sqlVolunteerRequest, sqlVolunteerRequestParameters);

        var volunteerRequests = await dbConnection.QueryAsync<VolunteerRequestDto>(
             sqlVolunteerRequest,
             sqlVolunteerRequestParameters
        );

        var pageResult = new PagedResult<VolunteerRequestDto>(
            volunteerRequests.ToList(),
            count,
            pageSize);

        return Result.Ok(pageResult);
    }
}
