using Dapper;
using Microsoft.Extensions.Logging;
using PetFamily.Application.Abstractions;
using PetFamily.VolunteerRequests.Application.IRepositories;
using PetFamily.VolunteerRequests.Domain.Entities;

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

        const string sql = @"
            SELECT COUNT(1) FROM volunteer_requests.volunteer_requests
            WHERE user_id = @UserId;
        ";

        var count = await connection.ExecuteScalarAsync<int>(sql, new { UserId = userId });
        return count > 0;
    }

    public Task<VolunteerRequest> GetByUserIdAsync(Guid userId, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
