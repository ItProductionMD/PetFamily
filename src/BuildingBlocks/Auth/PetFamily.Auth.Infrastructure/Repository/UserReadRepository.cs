using Dapper;
using Microsoft.Extensions.Logging;
using PetFamily.SharedApplication.Abstractions;
using PetFamily.Auth.Application.Dtos;
using PetFamily.Auth.Application.IRepositories;
using PetFamily.Auth.Domain.Entities.UserAggregate;
using PetFamily.Auth.Domain.ValueObjects;
using PetFamily.Auth.Public.Contracts;
using PetFamily.Auth.Public.Dtos;
using PetFamily.SharedInfrastructure.Dapper.ScaffoldedClasses;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.Validations;
using PetFamily.SharedKernel.ValueObjects;

namespace PetFamily.Auth.Infrastructure.Repository;

public class UserReadRepository(
    IDbConnectionFactory dbConnectionFactory,
    ILogger<UserReadRepository> logger) : IUserReadRepository, IUserContract
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
            {UsersTable.Email} AS Email,
            {UsersTable.Login} AS Login,
            {UsersTable.PhoneRegionCode} AS PhoneRegionCode,
            {UsersTable.PhoneNumber} AS PhoneNumber
        FROM {UsersTable.TableFullName}
        WHERE {UsersTable.Email} = @Email
            OR {UsersTable.Login} = @Login
            OR ({UsersTable.PhoneRegionCode} = @RegionCode AND {UsersTable.PhoneNumber} = @Number)
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
            {UsersTable.Id} As Id,           
            {UsersTable.Email} AS Email,
            {UsersTable.Login} AS Login,
        CONCAT({UsersTable.PhoneRegionCode},'-',{UsersTable.PhoneNumber}) AS Phone
        FROM {UsersTable.TableFullName}
        WHERE {UsersTable.Id} = @Id         
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
                u.{UsersTable.Id} AS Id,
                u.{UsersTable.Login} AS Login,
                u.{UsersTable.Email} AS Email,
                u.{UsersTable.IsEmailConfirmed} AS IsEmailConfirmed,
                CONCAT(COALESCE({UsersTable.PhoneRegionCode}, ''),'-',COALESCE({UsersTable.PhoneNumber}, '')) AS Phone,
                u.{UsersTable.IsBlocked} AS IsBlocked,
                u.{UsersTable.BlockedAt} AS BlockedAt,
                u.{UsersTable.CreatedAt} AS CreatedAt,
                u.{UsersTable.LastLoginDate} AS LastLoginDate,
                u.{UsersTable.UpdatedAt} AS UpdatedAt,
                json_agg(DISTINCT r.{RolesTable.Code} ORDER BY r.{RolesTable.Code}) AS Roles,
                json_agg(DISTINCT p.{PermissionsTable.Code} ORDER BY p.code) AS Permissions
                FROM {UsersTable.TableFullName} u
            LEFT JOIN 
                auth.user_role ur ON ur.user_id = u.{UsersTable.Id}
            LEFT JOIN 
                {RolesTable.TableName} r ON r.{RolesTable.Id} = ur.role_id
            LEFT JOIN 
                auth.role_permissions rp ON rp.role_id = r.{RolesTable.Id}
            LEFT JOIN 
                {PermissionsTable.TableFullName} p ON p.{PermissionsTable.Id} = rp.permission_id
            WHERE
                u.{UsersTable.Id} = @UserId
            GROUP BY 
                u.{UsersTable.Id},
                u.{UsersTable.Login}, 
                u.{UsersTable.Email}, 
                u.{UsersTable.IsEmailConfirmed},
                u.{UsersTable.IsBlocked},
                u.{UsersTable.BlockedAt},
                u.{UsersTable.CreatedAt},
                u.{UsersTable.LastLoginDate},
                u.{UsersTable.UpdatedAt};";

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

    Task<Result<UserDto>> IUserReadRepository.GetByIdAsync(Guid id, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    private class DuplicateRaw
    {
        public string? Email { get; set; }
        public string? Login { get; set; }
        public string? PhoneRegionCode { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
