using Dapper;
using PetFamily.Application.Abstractions;
using PetFamily.Auth.Application.Dtos;
using PetFamily.Auth.Application.IRepositories;
using PetFamily.SharedInfrastructure.Dapper.ScaffoldedClasses;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;

namespace PetFamily.Auth.Infrastructure.Repository;

public class PermissionReadRepository(
    IDbConnectionFactory dbConnectionFactory) : IPermissionReadRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory = dbConnectionFactory;

    public async Task<List<PermissionDto>> GetPermissionsAsync(CancellationToken ct = default)
    {
        await using var connection = await _dbConnectionFactory.CreateOpenConnectionAsync();

        var sql = $@"
            SELECT p.{PermissionsTable.Id} AS PermissionId,
                p.{PermissionsTable.Code} AS PermissionCode,
                p.{PermissionsTable.IsEnabled} As IsEnable
            FROM {PermissionsTable.TableName} p
            ORDER BY p.{PermissionsTable.Code}" ;

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
