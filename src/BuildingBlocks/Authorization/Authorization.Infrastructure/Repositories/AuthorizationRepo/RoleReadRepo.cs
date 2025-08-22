using Authorization.Application.Dtos;
using Authorization.Application.IRepositories.IAuthorizationRepo;
using Dapper;
using Microsoft.Extensions.Logging;
using PetFamily.SharedApplication.Abstractions;
using PetFamily.SharedInfrastructure.Dapper.ScaffoldedClasses;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.ValueObjects.Ids;

namespace Authorization.Infrastructure.Repositories.AuthorizationRepo;

public class RoleReadRepo(
    IDbConnectionFactory dbConnectionFactory,
    ILogger<RoleReadRepo> logger) : IRoleReadRepo
{

    public async Task<Result<RoleDto>> GetByCodeAsync(string roleCode, CancellationToken ct = default)
    {
        await using var connection = await dbConnectionFactory.CreateOpenConnectionAsync();

        var sql = $@"
        SELECT 
            r.{RolesTable.Id} AS RoleId, 
            r.{RolesTable.Code} AS RoleCode,   
            p.{PermissionsTable.Id} AS PermissionId,
            p.{PermissionsTable.Code} AS PermissionCode,
            p.{PermissionsTable.IsEnabled} AS IsEnable
        FROM {RolesTable.TableFullName} r
            JOIN {RolePermissionsTable.TableFullName} rp 
                ON r.{RolesTable.Id} = rp.{RolePermissionsTable.RoleId}
            JOIN {PermissionsTable.TableFullName} p 
                ON p.{PermissionsTable.Id} = rp.{RolePermissionsTable.PermissionId}
        WHERE r.{RolesTable.Code} = @RoleCode
        ORDER BY p.{PermissionsTable.Code};
    ";

        logger.LogInformation("EXECUTING QUERY: {sql}", sql);

        var raw = await connection.QueryAsync<RoleWithPermissionRaw>(sql, new { RoleCode = roleCode });

        if (!raw.Any())
            return Result.Fail(Error.NotFound($"Role with code '{roleCode}' not found"));

        var grouped = raw
        .GroupBy(x => new { x.RoleId, x.RoleCode })
        .Select(g => new RoleDto(
            g.Key.RoleId,
            g.Key.RoleCode,
            g
             .Where(p => p.PermissionId.HasValue)
             .Select(p =>
                new PermissionDto(
                    p.PermissionId.Value,
                    p.PermissionCode,
                    p.IsEnable.Value)
                ).ToList())
        ).First();

        return Result.Ok(grouped);
    }

    public Task<Result<RoleDto>> GetRoleByUserIdAsync(Guid userId, CancellationToken ct)
    {
        var sql = $@"
           SELECT 
               r.{RolesTable.Id} AS RoleId, 
               r.{RolesTable.Code} AS RoleCode,   
               p.{PermissionsTable.Id} AS PermissionId,
               p.{PermissionsTable.Code} AS PermissionCode,
               p.{PermissionsTable.IsEnabled} AS IsEnable
           FROM {RolesTable.TableFullName} r
               LEFT JOIN {RolePermissionsTable.TableFullName} rp 
                  ON r.{RolesTable.Id} = rp.{RolePermissionsTable.RoleId}
               LEFT JOIN {PermissionsTable.TableFullName} p 
                  ON p.{PermissionsTable.Id} = rp.{RolePermissionsTable.PermissionId}
               ORDER BY r.{RolesTable.Code}, p.{PermissionsTable.Code} ;";

        logger.LogInformation("EXECUTING QUERY: {sql}", sql);
        throw new NotImplementedException();
    }

    public async Task<List<RoleDto>> GetRoles(CancellationToken ct = default)
    {
        await using var connection = await dbConnectionFactory.CreateOpenConnectionAsync();

        var sql = $@"
           SELECT 
               r.{RolesTable.Id} AS RoleId, 
               r.{RolesTable.Code} AS RoleCode,   
               p.{PermissionsTable.Id} AS PermissionId,
               p.{PermissionsTable.Code} AS PermissionCode,
               p.{PermissionsTable.IsEnabled} AS IsEnable
           FROM {RolesTable.TableFullName} r
               LEFT JOIN {RolePermissionsTable.TableFullName} rp 
                  ON r.{RolesTable.Id} = rp.{RolePermissionsTable.RoleId}
               LEFT JOIN {PermissionsTable.TableFullName} p 
                  ON p.{PermissionsTable.Id} = rp.{RolePermissionsTable.PermissionId}
               ORDER BY r.{RolesTable.Code}, p.{PermissionsTable.Code} ;";

        logger.LogInformation("EXECUTING QUERY: {sql}", sql);


        var raw = await connection.QueryAsync<RoleWithPermissionRaw>(sql);

        var grouped = raw
        .GroupBy(x => new { x.RoleId, x.RoleCode })
        .Select(g => new RoleDto(
            g.Key.RoleId,
            g.Key.RoleCode,
            g
             .Where(p => p.PermissionId.HasValue)
             .Select(p =>
                new PermissionDto(
                    p.PermissionId.Value,
                    p.PermissionCode,
                    p.IsEnable.Value)
                ).ToList())
        ).ToList();

        return grouped;
    }
    public async Task<List<RoleDto>> GetRolesByUserId(UserId userId, CancellationToken ct)
    {
        await using var connection = await dbConnectionFactory.CreateOpenConnectionAsync();

        var sql = $@"
        SELECT 
            r.{RolesTable.Id} AS RoleId, 
            r.{RolesTable.Code} AS RoleCode,   
            p.{PermissionsTable.Id} AS PermissionId,
            p.{PermissionsTable.Code} AS PermissionCode,
            p.{PermissionsTable.IsEnabled} AS IsEnable
        FROM {RolesTable.TableFullName} r
        JOIN {UserRolesTable.TableFullName} u
            ON r.{RolesTable.Id} = u.{UserRolesTable.RoleId}
        LEFT JOIN {RolePermissionsTable.TableFullName} rp 
            ON r.{RolesTable.Id} = rp.{RolePermissionsTable.RoleId}
        LEFT JOIN {PermissionsTable.TableFullName} p 
            ON p.{PermissionsTable.Id} = rp.{RolePermissionsTable.PermissionId}
        WHERE u.{UserRolesTable.UserId} = @UserId;";

        logger.LogInformation("EXECUTING QUERY: {sql}", sql);

        var raw = await connection.QueryAsync<RoleWithPermissionRaw>(sql, new { UserId = userId.Value });

        if (!raw.Any())
            return new List<RoleDto>();

        var grouped = raw
            .GroupBy(x => new { x.RoleId, x.RoleCode })
            .Select(g => new RoleDto(
                g.Key.RoleId,
                g.Key.RoleCode,
                g
                 .Where(p => p.PermissionId.HasValue)
                 .Select(p =>
                     new PermissionDto(
                         p.PermissionId!.Value,
                         p.PermissionCode!,
                         p.IsEnable!.Value)
                 ).ToList())
            ).ToList();

        return grouped;
    }


    public async Task<UnitResult> VerifyRolesExist(IEnumerable<Guid> roleIds, CancellationToken ct = default)
    {
        var ids = roleIds.ToList();

        if (ids.Count == 0)
            return UnitResult.Ok();

        await using var connection = await dbConnectionFactory.CreateOpenConnectionAsync();

        var sql = $@"
        SELECT COUNT(*) 
        FROM {RolesTable.TableFullName} 
        WHERE {RolesTable.Id} = ANY(@Ids);";

        logger.LogInformation("EXECUTONG QUERY:{sql}", sql);

        var count = await connection.ExecuteScalarAsync<int>(sql, new { Ids = ids });

        if (count != ids.Count)
            return UnitResult.Fail(Error.NotFound("Some permission IDs do not exist."));


        return UnitResult.Ok();
    }
}
public class RoleWithPermissionRaw
{
    public Guid RoleId { get; set; }
    public string RoleCode { get; set; }
    public Guid? PermissionId { get; set; }
    public string? PermissionCode { get; set; }
    public bool? IsEnable { get; set; }
}
