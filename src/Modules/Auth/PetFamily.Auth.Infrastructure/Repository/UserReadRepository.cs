using Dapper;
using Microsoft.Extensions.Logging;
using PetFamily.Application.Abstractions;
using PetFamily.Auth.Application.Dtos;
using PetFamily.Auth.Application.IRepositories;
using PetFamily.Auth.Domain.Entities.UserAggregate;
using PetFamily.Auth.Domain.ValueObjects;
using PetFamily.SharedInfrastructure.Dapper.ScaffoldedClasses;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.Validations;
using PetFamily.SharedKernel.ValueObjects;

namespace PetFamily.Auth.Infrastructure.Repository;

public class UserReadRepository(
    IDbConnectionFactory dbConnectionFactory,
    ILogger<UserReadRepository> logger) : IUserReadRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory = dbConnectionFactory;
    private readonly ILogger<UserReadRepository> _logger = logger;

    public Task<bool> AnyUserWithRoleAsync(RoleId roleId, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public async Task<UnitResult> CheckUniqueFields(
        string email,
        string login,
        Phone phone,
        CancellationToken ct)
    {
        var sql = $@"
        SELECT 
            {UserTable.Email} AS Email,
            {UserTable.Login} AS Login,
            {UserTable.PhoneRegionCode} AS PhoneRegionCode,
            {UserTable.PhoneNumber} AS PhoneNumber
        FROM {UserTable.TableFullName}
        WHERE {UserTable.Email} = @Email
            OR {UserTable.Login} = @Login
            OR ({UserTable.PhoneRegionCode} = @RegionCode AND {UserTable.PhoneNumber} = @Number)
        LIMIT 4;";

        await using var connection = await _dbConnectionFactory.CreateOpenConnectionAsync();

        var matches = await connection.QueryAsync<DuplicateRaw>(sql, new
        {
            Email = email,
            Login = login,
            RegionCode = phone.RegionCode,
            Number = phone.Number
        });

        var errors = new List<ValidationError>();

        foreach (var row in matches)
        {
            if (row.Email == email)
                errors.AddRange(Error.ValueIsAlreadyExist("Email").ValidationErrors);

            if (row.Login == login)
                errors.AddRange(Error.ValueIsAlreadyExist("Login").ValidationErrors);

            if (row.PhoneRegionCode == phone.RegionCode && row.PhoneNumber == phone.Number)
                errors.AddRange(Error.ValueIsAlreadyExist("Phone").ValidationErrors);
        }

        return errors.Count > 0
            ? UnitResult.Fail(Error.FromValidationErrors(errors))
            : UnitResult.Ok();
    }

    public async Task<Result<UserDto>> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var sql = $@"
        SELECT 
            {UserTable.Id} As Id,           
            {UserTable.Email} AS Email,
            {UserTable.Login} AS Login,
        CONCAT({UserTable.PhoneRegionCode},'-',{UserTable.PhoneNumber}) AS Phone
        FROM {UserTable.TableFullName}
        WHERE {UserTable.Id} = @Id         
        LIMIT 1;";

        _logger.LogInformation("EXECURING QUERY:{sql} with params:{id}", sql, id);
        await using var connection = await _dbConnectionFactory.CreateOpenConnectionAsync();

        var user = await connection.QuerySingleOrDefaultAsync<UserDto>(sql, new { Id = id });

        if (user == null)
        {
            _logger.LogWarning("User with Id:{Id} not found!", id);
            return Result.Fail(Error.NotFound("User"));
        }
        return Result.Ok(user);
    }

    public async Task<Result<UserAccountInfoDto>> GetUserAccountInfo(Guid userId, CancellationToken ct = default)
    {
        var sql = $@"
            SELECT 
                u.{UserTable.Id} AS Id,
                u.{UserTable.Login} AS Login,
                u.{UserTable.Email} AS Email,
                u.{UserTable.IsEmailConfirmed} AS IsEmailConfirmed,
                CONCAT(COALESCE({UserTable.PhoneRegionCode}, ''),'-',COALESCE({UserTable.PhoneNumber}, '')) AS Phone,
                u.{UserTable.IsBlocked} AS IsBlocked,
                u.{UserTable.BlockedAt} AS BlockedAt,
                u.{UserTable.CreatedAt} AS CreatedAt,
                u.{UserTable.LastLoginDate} AS LastLoginDate,
                u.{UserTable.UpdatedAt} AS UpdatedAt,
                json_agg(DISTINCT r.{RoleTable.Code} ORDER BY r.{RoleTable.Code}) AS Roles,
                json_agg(DISTINCT p.{PermissionsTable.Code} ORDER BY p.code) AS Permissions
                FROM {UserTable.TableFullName} u
            LEFT JOIN 
                auth.user_role ur ON ur.user_id = u.{UserTable.Id}
            LEFT JOIN 
                {RoleTable.TableName} r ON r.{RoleTable.Id} = ur.role_id
            LEFT JOIN 
                auth.role_permissions rp ON rp.role_id = r.{RoleTable.Id}
            LEFT JOIN 
                {PermissionsTable.TableName} p ON p.{PermissionsTable.Id} = rp.permission_id
            WHERE
                u.{UserTable.Id} = @UserId
            GROUP BY 
                u.{UserTable.Id},
                u.{UserTable.Login}, 
                u.{UserTable.Email}, 
                u.{UserTable.IsEmailConfirmed},
                u.{UserTable.IsBlocked},
                u.{UserTable.BlockedAt},
                u.{UserTable.CreatedAt},
                u.{UserTable.LastLoginDate},
                u.{UserTable.UpdatedAt};";

        await using var connection = await _dbConnectionFactory.CreateOpenConnectionAsync();

        _logger.LogInformation("EXECUTING QUERY: {query}", sql);

        var userAccountInfo = await connection.QueryFirstOrDefaultAsync<UserAccountInfoDto>(
            sql,
            new {UserID = userId});

        if (userAccountInfo == null)
        {
            _logger.LogWarning("User account with id:{userId} not found!", userId);
            return Result.Fail(Error.NotFound("User"));
        }
        _logger.LogInformation("Get UserAccountInfo for user with id:{userId} successful!", userId);

        return Result.Ok(userAccountInfo);
    }

    private class DuplicateRaw
    {
        public string? Email { get; set; }
        public string? Login { get; set; }
        public string? PhoneRegionCode { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
