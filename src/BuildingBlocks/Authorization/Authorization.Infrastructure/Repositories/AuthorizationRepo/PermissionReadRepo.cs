using Authorization.Application.Dtos;
using Authorization.Application.IRepositories.IAuthorizationRepo;
using Dapper;
using PetFamily.SharedApplication.Abstractions;
using PetFamily.SharedInfrastructure.Dapper.ScaffoldedClasses;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;

namespace Authorization.Infrastructure.Repositories.AuthorizationRepo;

public class PermissionReadRepo(
    IDbConnectionFactory dbConnectionFactory) : IPermissionReadRepo
{
    private readonly IDbConnectionFactory _dbConnectionFactory = dbConnectionFactory;

    public async Task<List<PermissionDto>> GetPermissionsAsync(CancellationToken ct = default)
    {
        await using var connection = await _dbConnectionFactory.CreateOpenConnectionAsync();

        var sql = $@"
            SELECT p.{PermissionsTable.Id} AS PermissionId,
                p.{PermissionsTable.Code} AS PermissionCode,
                p.{PermissionsTable.IsEnabled} As IsEnable
            FROM {PermissionsTable.TableFullName} p
            ORDER BY p.{PermissionsTable.Code}";

        var permissions = await connection.QueryAsync<PermissionDto>(sql);

        return permissions.ToList();
    }

    public async Task<UnitResult> VerifyPermissionsExist(IEnumerable<Guid> permissionsIds, CancellationToken ct = default)
    {
        var ids = permissionsIds.ToList();

        if (ids.Count == 0)
            return UnitResult.Ok();

        await using var connection = await _dbConnectionFactory.CreateOpenConnectionAsync();

        var sql = $@"
        SELECT COUNT(*) 
        FROM auth.permissions 
        WHERE id = ANY(@Ids);";

        var count = await connection.ExecuteScalarAsync<int>(sql, new { Ids = ids });

        if (count != ids.Count)
        {
            return UnitResult.Fail(Error.NotFound("Some permission IDs do not exist."));
        }

        return UnitResult.Ok();
    }


}
