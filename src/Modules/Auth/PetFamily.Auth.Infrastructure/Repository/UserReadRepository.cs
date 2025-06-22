using Dapper;
using Microsoft.Extensions.Logging;
using PetFamily.Application.Abstractions;
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

    public async Task<UnitResult> CheckUniqueFields(string email, string login, Phone phone, CancellationToken ct)
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

    public Task<User?> GetByEmailAsync(string email, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<User?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public async Task<User?> GetByLoginAndPasswordAsync(
        string login,
        string hashedPassword,
        CancellationToken ct = default)
    {
        try
        {
            await using var connection = await _dbConnectionFactory.CreateOpenConnectionAsync();

            var query = "SELECT * FROM users" +
                " WHERE login = @Login " +
                "AND hashed_password = @HashedPassword";

            return await connection.QueryFirstOrDefaultAsync<User>(
            query, new { Login = login, HashedPassword = hashedPassword });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex, "Error occurred while getting user by login and password, login:{login}", login);

            return null;
        }
    }

    private class DuplicateRaw
    {
        public string? Email { get; set; }
        public string? Login { get; set; }
        public string? PhoneRegionCode { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
